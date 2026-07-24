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

  LoadRecordList: function () {
    const tx = window.gameDB.transaction(["Records"], "readonly");
    const store = tx.objectStore("Records");

    const request = store.openCursor();

    const records = [];

    request.onsuccess = function (e) {
      const cursor = e.target.result;

      if (cursor) {
        records.push({
          id: cursor.value.id,
          name: cursor.value.name,
        });

        cursor.continue();
      } else {
        // Unityへ一覧を返す
        const json = JSON.stringify({
          records: records,
        });

        SendMessage("DatabaseManager", "OnRecordListLoaded", json);
      }
    };

    request.onerror = function () {
      console.error("Record一覧取得失敗");
    };
  },
});
