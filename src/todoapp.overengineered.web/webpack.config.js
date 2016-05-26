var path = require('path');

var webpackConfig = {
    context: __dirname + path.sep + 'scripts',
    entry: "./app.js",
    output: {
        path: __dirname + "/../wwwroot/dist/js",
        filename: "bundle.js"
    },
    resolve: {root: [__dirname + path.sep + 'scripts']},
    module: {
        loaders: [
            { test: /\.js$/, loader: "babel-loader" },
            { test: /\.css$/, loader: "style-loader!css-loader" }
        ]
    }
};

module.exports = webpackConfig;