import request from 'superagent';
import superagentPromisePlugin from 'superagent-promise-plugin';
superagentPromisePlugin.Promise = Promise;


export function query(url) {

    return new Promise((resolve, reject) => {
        request.get(url)
            .use(superagentPromisePlugin)
            .then((result) => resolve(result.body), (error) => reject(error));
    });
}