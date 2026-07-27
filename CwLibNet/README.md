# CWLibNet

CWLibNet is a port of the CWLib library to .NET.

CWLib is a library for reading and writing data from the LittleBigPlanet game series, created by Media Molecule (which the developers of CWLibNet are not affiliated with).
The original Java library has been developed by [ennuo](https://github.com/ennuo) and is available under the MIT License.

CWLibNet is for now **incomplete** and **not fully tested**, does not read all kinds of data, and may contain bugs. It is still in development and will be updated over time.

For now it can serialize and deserialize generic assets, read and write FARCs and MAPs, and many structs and resource types have been ported. It also works with Textures and handles Compression using ZLib.

There is some code which is taken from a Java library called jsqush, which has a separated README and LICENSE (see Squish directory). The code is used for decompressing and compressing DXT textures.