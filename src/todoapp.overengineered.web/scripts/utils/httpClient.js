import jquery from 'jquery';
import polly from 'polly-js';
import {addAjaxError} from 'errorHandler';

var retryableStatusCodes = [408, 429, 502, 503, 504];

export function downloadFile(url, message, method, fileName, contentType, acceptContentType) {
    var xhr = new XMLHttpRequest();
    xhr.open(method, url);
    if (acceptContentType) {
        xhr.setRequestHeader('Accept', acceptContentType);
    }
    if (contentType) {
        xhr.setRequestHeader('content-type', contentType);
    }
    xhr.responseType = 'blob';
    xhr.onload = function() {
        if (this.status >= 400) {
            addAjaxError(xhr, url, 'post');
        } else {
            if (window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(xhr.response, fileName);
            } else {
                var objectUrl = URL.createObjectURL(xhr.response);
                window.location.href = objectUrl;
            }
        }
    };
    xhr.send(message);
}

export function post(url, message) {
    if (url.indexOf('#') !== -1) {
        console.error('Requesting a resource with a non encoded URL:', url);
    }

    return polly()
        .handle(res => {
            return retryableStatusCodes.indexOf(res.status) !== -1;
        })
        .waitAndRetry([50, 100, 200])
        .executeForPromise(() =>
            jquery.ajax({
                method: "POST",
                url: url,
                data: JSON.stringify(message),
                contentType: 'application/json',
                dataType: 'json'
            })
        )
        .catch(jqxhr => {
            if (jqxhr.status !== 404) {
                addAjaxError(jqxhr, url, 'getJSON');
            }
            return jqxhr;
        });
}

export function query(url) {
    if (url.indexOf('#') !== -1) {
        console.error('Requesting a resource with a non encoded URL:', url);
    }

    return polly()
        .handle(res => {
            return retryableStatusCodes.indexOf(res.status) !== -1;
        })
        .waitAndRetry([50, 100, 200])
        .executeForPromise(() =>
            jquery.getJSON(url)
                .then((data, status, xhr) => {
                    if (data && typeof data === 'object') {
                        data.__etag = xhr.getResponseHeader('etag');
                    }

                    return data;
                })
        )
        .catch(jqxhr => {
            if (jqxhr.status !== 404) {
                addAjaxError(jqxhr, url, 'getJSON');
            }
            return jqxhr;
        });
}


export function uploadBinary(name, file, url) {
    url = url || 'api/blobs';
    var formData = new FormData();
    formData.append(name, file);

    return jquery.ajax({
        url: url,
        type: 'POST',
        data: formData,
        async: true,
        cache: false,
        processData: false,
        contentType: false
    });
};

export default {
    downloadFile,
    post,
    query,
    uploadBinary
}