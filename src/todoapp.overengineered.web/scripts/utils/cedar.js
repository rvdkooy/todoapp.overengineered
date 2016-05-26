import request from 'superagent';
import superagentPromisePlugin from 'superagent-promise-plugin';
superagentPromisePlugin.Promise = Promise;


export function execute(command, options) {
    let version = '';
    options = options || {
        baseUrl: ''
    };

    if (command.version) {
        version = '.v' + command.version;
    }

    const url = options.baseUrl + 'commands/' + command.commandId;

    return  request.put(url)
                    .send(JSON.stringify(command))
                    .set('Accept', '"application/problem+json; charset=utf-8"')
                    .set('Content-Type', 'application/vnd.' + command.commandName + version + '+json')
                    .use(superagentPromisePlugin)
                    .then((success) => {
                        console.log(success);
                    }, (error) => {
                        console.log(error);
                    });
};
