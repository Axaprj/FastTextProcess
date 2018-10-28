--
-- File generated with SQLiteStudio v3.2.1 on Вс окт 28 15:27:22 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Dict
CREATE TABLE Dict (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT UNIQUE NOT NULL, Vect BLOB NOT NULL);

-- Index: inxWord
CREATE INDEX inxWord ON Dict (Word);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
