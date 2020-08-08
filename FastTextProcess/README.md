# Axaprj.FastTextProcess
Text to embedded vectors conversion high-performance framework ([Facebook FastText](https://fasttext.cc) engine and dictionaries based).
- uses [Axaprj.WordToVecDB](https://github.com/Axaprj/FastTextProcess/tree/master/Axaprj.WordToVecDB) DB access layer of embedded vectors processing tools. 
- a part of [FastTextProcess](https://github.com/Axaprj/FastTextProcess) project.
 
## code
VS 2017, VS 2019, .NET Core, SQLite, XUnit

## toolchain and data (currently used)
- [FastText pre-trained word vectors model](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.bin.gz) - cc.en.300.bin.gz extract into `FastTextDir`
- [FastText pre-trained word vectors dictionary](https://s3-us-west-1.amazonaws.com/fasttext-vectors/cc.en.300.vec.gz) - cc.en.300.vec.gz extract into `FastTextDir`
- [Large Movie Review Dataset v1.0](https://ai.stanford.edu/~amaas/data/sentiment/) - aclImdb_v1.tar.gz demo data, extract into `DataArcDir`
- own [FastText VS 2017 CMake compatibility fix](https://github.com/Axaprj/fastText) - any other valid build can be used
- own [cnn-text-classification-tf](https://github.com/Axaprj/cnn-text-classification-tf/blob/master/vocab_process_ft.3.py) - preprocessed data loader demo of toolchain connection to Python TensorFlow

## start of usage 
- setup `FastTextProcess.Tests.json`
```json
{
  "DataArcDir": "c:/data.arc/",
  "DataOutDir": "C:/data.arc/statDb/",
  "FastTextDir": "c:/data.ft/",
  "FastTextBin": "c:/data.ft/fasttext.exe"
}
```
- Create pretrained vectors DB [FastTextProcess.Tests.AclImdbDemo](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/AclImdbDemo.cs)
```c#
ProcCreateDbEn()
```
- Append Train and Tests  AclImdb Data [FastTextProcess.Tests.AclImdbDemo](https://github.com/Axaprj/FastTextProcess/blob/master/Tests/FastTextProcess.Tests/AclImdbDemo.cs)
```c#
ProcAclImdbTrain()
ProcAclImdbTest()
```
- Looking for result into `$DataOutDir/AclImdb_proc.db` and `$DataOutDir/w2v_en.db` dictionary DB

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
```
[4] Large Movie Review Dataset v1.0 @InProceedings{maas-EtAl:2011:ACL-HLT2011,author={Maas, Andrew L. and Daly, Raymond E. and Pham, Peter T. and Huang, Dan and Ng, Andrew Y. and Potts, Christopher},title={Learning Word Vectors for Sentiment Analysis},booktitle = {Proceedings of the 49th Annual Meeting of the Association for Computational Linguistics: Human Language Technologies},month={June},year={2011},address={Portland, Oregon, USA},publisher ={Association for Computational Linguistics},pages={142--150},url={http://www.aclweb.org/anthology/P11-1015}}
```

## authors
[Axaprj](https://github.com/Axaprj), [Igor Alexeev](mailto:axaprj2000@yahoo.com) 

You are welcome to [Property Indicators Lab](https://propertyindicators.github.io/)! 
We know how to use it in real projects.
For any questions, please contact us at email propertyindicators@gmail.com.
