const { when, whenDev, whenProd, whenCI, whenTest, ESLINT_MODES, POSTCSS_MODES } = require('@craco/craco');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CracoAntDesignPlugin = require('craco-antd');

module.exports = {
  plugins: [
    {
      plugin: CracoAntDesignPlugin,
      options: {
        customizeTheme: {
          '@primary-color': '#2680EB',
          '@link-color': '#2680EB',
        },
      },
    },
  ],
  webpack: {
    alias: {},
    plugins: [],
    configure: (webpackConfig, { env, paths }) => {
      if (!webpackConfig.plugins) {
        config.plugins = [];
      }

      webpackConfig.plugins.push(
        process.env.NODE_ENV === 'production'
          ? new CopyWebpackPlugin([
              {
                from: 'node_modules/@aspnet/signalr/dist/browser/signalr.min.js',
              },
              {
                from: 'node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js',
              },
              {
                from: 'src/lib/abp.js',
              },
            ])
          : new CopyWebpackPlugin([
              {
                from: 'node_modules/@aspnet/signalr/dist/browser/signalr.min.js',
              },
              {
                from: 'node_modules/abp-web-resources/Abp/Framework/scripts/libs/abp.signalr-client.js',
              },
              {
                from: 'src/lib/abp.js',
              },
            ])
      );

      return webpackConfig;
    },
  },
};
