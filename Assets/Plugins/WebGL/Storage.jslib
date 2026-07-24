mergeInto(LibraryManager.library, {
  OpenDatabase: function () {
    const request = indexedDB.open("GameDB", 1);

    request.onupgradeneeded = function (e) {
      const db = e.target.result;

      if (!db.objectStoreNames.contains("Records")) {
        db.createObjectStore("Records", {
          keyPath: "id",
        });
      }
    };

    request.onsuccess = function (e) {
      window.gameDB = e.target.result;
    };
  },

  SaveRecord: function (jsonPtr) {
    const json = UTF8ToString(jsonPtr);

    const record = JSON.parse(json);

    const tx = window.gameDB.transaction(["Records"], "readwrite");

    tx.objectStore("Records").put(record);
  },

  LoadRecord: function (idPtr) {
    const id = UTF8ToString(idPtr);

    const tx = window.gameDB.transaction(["Records"], "readonly");

    const request = tx.objectStore("Records").get(id);

    request.onsuccess = function () {
      if (request.result) {
        const json = JSON.stringify(request.result);

        // Unityへ返す処理
        SendMessage("DatabaseManager", "OnRecordLoaded", json);
      }
    };
  },
});
