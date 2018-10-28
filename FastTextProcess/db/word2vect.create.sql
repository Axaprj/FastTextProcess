--
-- File generated with SQLiteStudio v3.2.1 on Вс окт 28 21:09:41 2018
--
-- Text encoding used: System
--
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: Dict
CREATE TABLE Dict (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT UNIQUE NOT NULL, Vect BLOB NOT NULL);

-- Table: DictAddins
CREATE TABLE DictAddins (Id INTEGER PRIMARY KEY AUTOINCREMENT, Word TEXT UNIQUE NOT NULL, Vect BLOB NOT NULL);

-- Table: EmbedDict
CREATE TABLE EmbedDict ("Index" INTEGER UNIQUE PRIMARY KEY NOT NULL, DictId INTEGER REFERENCES Dict (Id) ON DELETE SET NULL ON UPDATE CASCADE, DictAddinsId INTEGER REFERENCES DictAddins (Id) ON DELETE SET NULL ON UPDATE CASCADE);

-- Index: inxWordDict
CREATE INDEX inxWordDict ON Dict (Word);

-- Index: inxWordDictAddins
CREATE INDEX inxWordDictAddins ON DictAddins (
    Word
);

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
