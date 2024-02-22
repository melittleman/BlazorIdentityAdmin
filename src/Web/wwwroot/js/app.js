window.isBraveBrowser = () => {

    return typeof (navigator.brave) !== 'undefined'
        ? window.navigator.brave?.isBrave()
        : false;
};
