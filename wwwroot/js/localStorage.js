window.localStorageHelper = {
    // データを保存
    setItem: function (key, value) {
        localStorage.setItem(key, value);
    },

    // データを取得
    getItem: function (key) {
        return localStorage.getItem(key);
    },
};
