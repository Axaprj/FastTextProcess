# FastTextProcess
Text to embedded vectors conversion high-performance framework.
[Facebook FastText](https://fasttext.cc) engine and dictionaries based.
 
## code
VS 2017, .NET Core, SQLite, XUnit

## toolchain and data (currently used)
- [FastText pre-trained word vectors model](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.bin.gz) extract into `DataArcDir`
- [FastText pre-trained word vectors dictionary](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.vec.gz) extract into `DataArcDir`
- [Large Movie Review Dataset v1.0](https://www.kaggle.com/iarunava/imdb-movie-reviews-dataset) extract into `DataArcDir`
- own [FastText VS 2017 CMake compatibility fix](https://github.com/Axaprj/fastText)
- own [cnn-text-classification-tf preprocessed data loader](https://github.com/Axaprj/cnn-text-classification-tf/blob/master/vocab_process_ft.3.py)

## start of usage 
- setup `FastTextProcess.Tests.json`
```json
{
  "DataArcDir": "./../../../../../../data.arc/",
  "FastTextBin": "./../../../../../../data.arc/fasttext.exe"
}
```
- Create pretrained vectors DB [FastTextProcess.Tests.FastTextRoutines](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/FastTextRoutines.cs)
```c#
ProcCreateDbEn()
```
- Append Train and Tests  AclImdb Data [FastTextProcess.Tests.FastTextRoutines](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/FastTextRoutines.cs)
```c#
ProcAclImdbTrain()
ProcAclImdbTest()
```
- Looking for result into `$DataArcDir/AclImdb_proc.db`

### Igor Alexeev, axaprj2000@yahoo.com for https://propertyindicators.github.io/
