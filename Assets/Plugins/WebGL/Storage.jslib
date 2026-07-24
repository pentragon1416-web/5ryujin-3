mergeInto(LibraryManager.library, {
  OpenDatabase: async function () {
    try {
      window.gameDB = await new Promise((resolve, reject) => {
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
          resolve(e.target.result);
        };

        request.onerror = function () {
          reject(request.error);
        };
      });

      console.log("データベースオープン成功");
      SendMessage("DatabaseManager", "OnDatabaseOpened");
    } catch (e) {
      console.error("データベースオープン失敗", e);
    }
  },

  SaveRecord: async function (jsonPtr) {
    const json = UTF8ToString(jsonPtr);
    const record = JSON.parse(json);

    try {
      await new Promise((resolve, reject) => {
        const tx = window.gameDB.transaction(["Records"], "readwrite");

        tx.objectStore("Records").put(record);

        tx.oncomplete = function () {
          resolve();
        };

        tx.onerror = function () {
          reject(tx.error);
        };

        tx.onabort = function () {
          reject(tx.error);
        };
      });

      console.log("保存完了");
      SendMessage("DatabaseManager", "OnRecordSaved");
    } catch (e) {
      console.error("保存失敗", e);
      SendMessage("DatabaseManager", "OnRecordSaveFailed");
    }
  },

  LoadRecord: async function (idPtr) {
    const id = UTF8ToString(idPtr);

    try {
      const record = await new Promise((resolve, reject) => {
        const tx = window.gameDB.transaction(["Records"], "readonly");

        const request = tx.objectStore("Records").get(id);

        request.onsuccess = function () {
          resolve(request.result);
        };

        request.onerror = function () {
          reject(request.error);
        };
      });

      if (record) {
        const json = JSON.stringify(record);
        SendMessage("DatabaseManager", "OnRecordLoaded", json);
      } else {
        console.log("レコードが見つかりません");
        SendMessage("DatabaseManager", "OnRecordNotFound");
      }
    } catch (e) {
      console.error("読み込み失敗", e);
      SendMessage("DatabaseManager", "OnRecordLoadFailed");
    }
  },

  LoadRecordList: async function () {
    try {
      const records = await new Promise((resolve, reject) => {
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
            // 全件取得完了
            resolve(records);
          }
        };

        request.onerror = function () {
          reject(request.error);
        };
      });

      const json = JSON.stringify({
        records: records,
      });

      SendMessage("DatabaseManager", "OnRecordListLoaded", json);
    } catch (e) {
      console.error("Record一覧取得失敗", e);
      SendMessage("DatabaseManager", "OnRecordListLoadFailed");
    }
  },
});
