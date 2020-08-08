# Axaprj.WordToVecDB
Entity-style DB access layer of embedded vectors processing tools. 
A part of [FastTextProcess](https://github.com/Axaprj/FastTextProcess) project.
- Extendable the word to vector dictionary database.
- Word to vector processing output database.

## word2vect (SQLite)
Word to vector dictionary.
#### Dict - pretrained dictionary
#### DictAddins - dictionary of macros and words not found in Dict
#### EmbedDict - Dict and DictAddins join to build a zero-based index of vectors

## vect_result (SQLite)
Custom text word to vector processing result.
#### Dict - vectors dictionary
- Index (zero-based)
- Vector (text representation)
#### Src - processed samples storage
- OriginalId (source sample key)
- ProcInfo (data for trainig)
- DictInxsStr (array of Dict.Index, text representation) - text sample representation ready to process 

## code
VS 2017, VS 2019, .NET Core, SQLite

## authors
[Axaprj](https://github.com/Axaprj), [Igor Alexeev](mailto:axaprj2000@yahoo.com) 

You are welcome to [Property Indicators Lab](https://propertyindicators.github.io/)! 
We know how to use it in real projects.
For any questions, please contact us at email propertyindicators@gmail.com.
