--
-- File generated with SQLiteStudio v3.2.1 on бс эюџ 24 15:46:30 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Dict
CREATE TABLE Dict (Inx INTEGER PRIMARY KEY, VectStr TEXT NOT NULL);

-- Table: Src
CREATE TABLE Src (OriginalId TEXT NOT NULL UNIQUE PRIMARY KEY, ProcInfo TEXT, DbgInfo TEXT, DictInxsStr TEXT NOT NULL);

-- Index: inxSrcOriginalId
CREATE INDEX inxSrcOriginalId ON Src (OriginalId);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
