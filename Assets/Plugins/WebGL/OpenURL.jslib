mergeInto(LibraryManager.library, {
  OpenNewTab: function (url) {
    const str = UTF8ToString(url);
    console.log("OpenNewTab:", str);
    window.open(str, "_blank");
  },
});
