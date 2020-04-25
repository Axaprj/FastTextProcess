# FastTextProcess
Natural Language Processing feature extraction multilingual tool set. 
(in the development phase now)
- [Axaprj.FastTextProcess](https://github.com/Axaprj/FastTextProcess/tree/master/FastTextProcess) Text to embedded vectors conversion high-performance framework ([Facebook FastText](https://fasttext.cc) engine and dictionaries based).
- [Axaprj.Textc.Vect](https://github.com/Axaprj/FastTextProcess/tree/master/Axaprj.Textc.Vect) Takenet.Textc  extension. 
- [Axaprj.WordToVecDB](https://github.com/Axaprj/FastTextProcess/tree/master/Axaprj.WordToVecDB) DB access layer of embedded vectors processing tools. 
 
## code
VS 2017, .NET Core, SQLite, XUnit

## toolchain and data (currently used)
- [FastText pre-trained word vectors model](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.bin.gz) extract into `DataArcDir`
- [FastText pre-trained word vectors dictionary](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.vec.gz) extract into `DataArcDir`
- [Large Movie Review Dataset v1.0](https://www.kaggle.com/iarunava/imdb-movie-reviews-dataset) extract into `DataArcDir`
- own [FastText VS 2017 CMake compatibility fix](https://github.com/Axaprj/fastText)
- own [cnn-text-classification-tf preprocessed data loader](https://github.com/Axaprj/cnn-text-classification-tf/blob/master/vocab_process_ft.3.py)
- own [Takenet.Textc .Net Core port](https://github.com/Axaprj/textc-csharp/tree/port2core)

## start of usage 
- setup `FastTextProcess.Tests.json`
```json
{
  "DataArcDir": "c:/data.arc/",
  "FastTextBin": "c:/data.arc/fasttext.exe"
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

## advanced and experimental
FastTextRoutinesCyr - multilingual processing: languages detection and Ru, Uk, En texts processor. 
[Aligned word vectors](https://fasttext.cc/docs/en/aligned-vectors.html) used.

## reference 
```
[1] T. Mikolov, E. Grave, P. Bojanowski, C. Puhrsch, A. Joulin. Advances in Pre-Training Distributed Word Representations @inproceedings{mikolov2018advances, title={Advances in Pre-Training Distributed Word Representations}, author={Mikolov, Tomas and Grave, Edouard and Bojanowski, Piotr and Puhrsch, Christian and Joulin, Armand}, booktitle={Proceedings of the International Conference on Language Resources and Evaluation (LREC 2018)}, year={2018}}
```
```
[2] A. Joulin, P. Bojanowski, T. Mikolov, H. Jegou, E. Grave, Loss in Translation: Learning Bilingual Word Mapping with a Retrieval Criterion @InProceedings{joulin2018loss, title={Loss in Translation: Learning Bilingual Word Mapping with a Retrieval Criterion}, author={Joulin, Armand and Bojanowski, Piotr and Mikolov, Tomas and J\'egou, Herv\'e and Grave, Edouard}, booktitle={Proceedings of the 2018 Conference on Empirical Methods in Natural Language Processing}, year={2018}}
```
```
[3] P. Bojanowski*, E. Grave*, A. Joulin, T. Mikolov, Enriching Word Vectors with Subword Information @article{bojanowski2017enriching, title={Enriching Word Vectors with Subword Information}, author={Bojanowski, Piotr and Grave, Edouard and Joulin, Armand and Mikolov, Tomas}, journal={Transactions of the Association for Computational Linguistics}, volume={5}, year={2017}, issn={2307-387X}, pages={135--146}}
```
## author
[Axaprj](https://github.com/Axaprj), [Igor Alexeev](axaprj2000@yahoo.com) for https://propertyindicators.github.io/
