using System.Diagnostics;
using CwLibNet.Enums;
using CwLibNet.Extensions;
using CwLibNet.Resources;
using CwLibNet.Util;
using SkiaSharp;

namespace CwLibNetTests;

public class ImagesTests
{
    private static readonly byte[] image =
    [
        137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82,
        0, 0, 0, 16, 0, 0, 0, 16, 8, 2, 0, 0, 0, 144, 145, 104,
        54, 0, 0, 0, 4, 103, 65, 77, 65, 0, 0, 177, 143, 11, 252, 97,
        5, 0, 0, 0, 32, 99, 72, 82, 77, 0, 0, 122, 38, 0, 0, 128,
        132, 0, 0, 250, 0, 0, 0, 128, 232, 0, 0, 117, 48, 0, 0, 234,
        96, 0, 0, 58, 152, 0, 0, 23, 112, 156, 186, 81, 60, 0, 0, 0,
        80, 101, 88, 73, 102, 77, 77, 0, 42, 0, 0, 0, 8, 0, 2, 1,
        18, 0, 3, 0, 0, 0, 1, 0, 1, 0, 0, 135, 105, 0, 4, 0,
        0, 0, 1, 0, 0, 0, 38, 0, 0, 0, 0, 0, 3, 160, 1, 0,
        3, 0, 0, 0, 1, 0, 1, 0, 0, 160, 2, 0, 4, 0, 0, 0,
        1, 0, 0, 0, 16, 160, 3, 0, 4, 0, 0, 0, 1, 0, 0, 0,
        16, 0, 0, 0, 0, 38, 136, 95, 236, 0, 0, 2, 48, 105, 84, 88,
        116, 88, 77, 76, 58, 99, 111, 109, 46, 97, 100, 111, 98, 101, 46, 120,
        109, 112, 0, 0, 0, 0, 0, 60, 120, 58, 120, 109, 112, 109, 101, 116,
        97, 32, 120, 109, 108, 110, 115, 58, 120, 61, 34, 97, 100, 111, 98, 101,
        58, 110, 115, 58, 109, 101, 116, 97, 47, 34, 32, 120, 58, 120, 109, 112,
        116, 107, 61, 34, 88, 77, 80, 32, 67, 111, 114, 101, 32, 54, 46, 48,
        46, 48, 34, 62, 10, 32, 32, 32, 60, 114, 100, 102, 58, 82, 68, 70,
        32, 120, 109, 108, 110, 115, 58, 114, 100, 102, 61, 34, 104, 116, 116, 112,
        58, 47, 47, 119, 119, 119, 46, 119, 51, 46, 111, 114, 103, 47, 49, 57,
        57, 57, 47, 48, 50, 47, 50, 50, 45, 114, 100, 102, 45, 115, 121, 110,
        116, 97, 120, 45, 110, 115, 35, 34, 62, 10, 32, 32, 32, 32, 32, 32,
        60, 114, 100, 102, 58, 68, 101, 115, 99, 114, 105, 112, 116, 105, 111, 110,
        32, 114, 100, 102, 58, 97, 98, 111, 117, 116, 61, 34, 34, 10, 32, 32,
        32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 120, 109, 108, 110, 115, 58,
        101, 120, 105, 102, 61, 34, 104, 116, 116, 112, 58, 47, 47, 110, 115, 46,
        97, 100, 111, 98, 101, 46, 99, 111, 109, 47, 101, 120, 105, 102, 47, 49,
        46, 48, 47, 34, 10, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32, 32,
        32, 120, 109, 108, 110, 115, 58, 116, 105, 102, 102, 61, 34, 104, 116, 116,
        112, 58, 47, 47, 110, 115, 46, 97, 100, 111, 98, 101, 46, 99, 111, 109,
        47, 116, 105, 102, 102, 47, 49, 46, 48, 47, 34, 62, 10, 32, 32, 32,
        32, 32, 32, 32, 32, 32, 60, 101, 120, 105, 102, 58, 80, 105, 120, 101,
        108, 89, 68, 105, 109, 101, 110, 115, 105, 111, 110, 62, 55, 56, 60, 47,
        101, 120, 105, 102, 58, 80, 105, 120, 101, 108, 89, 68, 105, 109, 101, 110,
        115, 105, 111, 110, 62, 10, 32, 32, 32, 32, 32, 32, 32, 32, 32, 60,
        101, 120, 105, 102, 58, 67, 111, 108, 111, 114, 83, 112, 97, 99, 101, 62,
        49, 60, 47, 101, 120, 105, 102, 58, 67, 111, 108, 111, 114, 83, 112, 97,
        99, 101, 62, 10, 32, 32, 32, 32, 32, 32, 32, 32, 32, 60, 101, 120,
        105, 102, 58, 80, 105, 120, 101, 108, 88, 68, 105, 109, 101, 110, 115, 105,
        111, 110, 62, 56, 50, 60, 47, 101, 120, 105, 102, 58, 80, 105, 120, 101,
        108, 88, 68, 105, 109, 101, 110, 115, 105, 111, 110, 62, 10, 32, 32, 32,
        32, 32, 32, 32, 32, 32, 60, 116, 105, 102, 102, 58, 79, 114, 105, 101,
        110, 116, 97, 116, 105, 111, 110, 62, 49, 60, 47, 116, 105, 102, 102, 58,
        79, 114, 105, 101, 110, 116, 97, 116, 105, 111, 110, 62, 10, 32, 32, 32,
        32, 32, 32, 60, 47, 114, 100, 102, 58, 68, 101, 115, 99, 114, 105, 112,
        116, 105, 111, 110, 62, 10, 32, 32, 32, 60, 47, 114, 100, 102, 58, 82,
        68, 70, 62, 10, 60, 47, 120, 58, 120, 109, 112, 109, 101, 116, 97, 62,
        10, 39, 142, 67, 183, 0, 0, 1, 193, 73, 68, 65, 84, 40, 21, 77,
        82, 65, 142, 219, 48, 12, 228, 74, 20, 35, 91, 113, 154, 44, 22, 1,
        114, 104, 63, 215, 55, 244, 212, 199, 246, 88, 160, 64, 155, 133, 99, 199,
        10, 101, 203, 29, 89, 233, 162, 4, 33, 147, 226, 12, 73, 145, 126, 249,
        250, 237, 251, 49, 4, 50, 121, 252, 115, 133, 18, 209, 151, 243, 27, 206,
        42, 150, 215, 106, 232, 61, 201, 167, 3, 108, 246, 206, 61, 131, 68, 225,
        245, 4, 251, 218, 247, 239, 183, 30, 198, 177, 59, 52, 171, 117, 141, 147,
        182, 96, 198, 97, 12, 251, 192, 206, 101, 141, 41, 191, 244, 182, 41, 68,
        223, 120, 233, 132, 131, 32, 252, 88, 83, 227, 109, 90, 147, 16, 73, 75,
        233, 209, 143, 67, 207, 203, 195, 86, 92, 156, 98, 97, 108, 114, 190, 156,
        241, 29, 111, 163, 75, 83, 189, 9, 193, 157, 95, 207, 63, 126, 254, 98,
        248, 118, 183, 212, 91, 164, 7, 77, 24, 25, 11, 58, 116, 193, 61, 230,
        127, 4, 185, 94, 199, 227, 62, 24, 160, 161, 190, 245, 151, 207, 151, 26,
        123, 34, 186, 240, 191, 59, 142, 90, 93, 14, 121, 116, 179, 17, 142, 52,
        188, 119, 76, 93, 71, 66, 147, 165, 217, 110, 117, 60, 201, 164, 26, 53,
        1, 157, 169, 112, 240, 104, 35, 130, 103, 100, 56, 123, 79, 58, 83, 142,
        37, 156, 226, 221, 176, 91, 179, 3, 58, 166, 114, 35, 165, 125, 226, 253,
        94, 134, 65, 137, 41, 205, 164, 92, 110, 157, 111, 129, 70, 204, 178, 67,
        82, 47, 207, 185, 231, 181, 84, 48, 170, 75, 74, 25, 232, 42, 168, 176,
        204, 37, 31, 104, 245, 230, 163, 66, 117, 249, 119, 180, 100, 237, 146, 202,
        112, 45, 250, 199, 42, 140, 198, 89, 189, 110, 118, 94, 176, 164, 18, 97,
        175, 83, 114, 59, 183, 245, 181, 113, 151, 89, 161, 197, 116, 104, 67, 26,
        41, 207, 45, 252, 198, 227, 252, 216, 146, 137, 247, 178, 47, 235, 36, 111,
        157, 148, 88, 74, 81, 117, 27, 142, 98, 220, 167, 183, 83, 69, 35, 189,
        160, 66, 117, 140, 93, 48, 19, 160, 33, 248, 23, 65, 128, 162, 78, 77,
        87, 23, 138, 144, 62, 18, 123, 246, 148, 232, 62, 165, 246, 80, 8, 178,
        179, 54, 235, 161, 21, 43, 229, 13, 211, 112, 93, 208, 24, 26, 111, 232,
        150, 130, 97, 50, 237, 33, 64, 221, 206, 84, 52, 78, 76, 173, 162, 117,
        24, 130, 127, 150, 117, 108, 128, 134, 252, 5, 168, 193, 228, 159, 168, 46,
        221, 191, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130
    ];

    private static readonly byte[] tex =
    [
        84, 69, 88, 32, 0, 1, 0, 1, 1, 27, 1, 244, 120, 218, 115, 113,
        9, 86, 168, 97, 96, 96, 96, 23, 96, 98, 16, 0, 210, 32, 12, 3,
        172, 12, 164, 1, 5, 32, 102, 1, 98, 151, 136, 16, 83, 108, 242, 28,
        2, 14, 152, 130, 172, 255, 193, 64, 160, 196, 55, 249, 240, 161, 21, 93,
        48, 62, 127, 205, 155, 168, 165, 85, 77, 76, 112, 126, 15, 79, 242, 170,
        175, 95, 87, 192, 248, 6, 61, 189, 197, 203, 167, 199, 123, 195, 248, 125,
        61, 239, 139, 67, 117, 215, 174, 134, 241, 253, 123, 206, 101, 155, 155, 107,
        45, 128, 241, 39, 246, 188, 109, 254, 249, 235, 85, 45, 92, 126, 202, 218,
        234, 79, 191, 254, 127, 133, 241, 243, 128, 250, 255, 124, 11, 13, 133, 241,
        253, 122, 206, 103, 55, 173, 122, 191, 21, 206, 159, 194, 95, 179, 250, 85,
        233, 119, 24, 159, 175, 229, 108, 245, 175, 159, 19, 167, 192, 221, 87, 146,
        155, 124, 120, 237, 252, 13, 48, 126, 65, 203, 185, 236, 171, 79, 247, 218,
        195, 248, 27, 167, 188, 43, 86, 223, 251, 235, 29, 140, 175, 215, 114, 190,
        88, 127, 221, 143, 15, 48, 190, 126, 141, 95, 114, 212, 42, 37, 120, 120,
        232, 245, 248, 102, 239, 125, 223, 176, 0, 73, 125, 54, 199, 244, 171, 166,
        48, 254, 132, 158, 119, 213, 113, 225, 23, 235, 224, 250, 91, 250, 178, 175,
        55, 116, 117, 51, 176, 218, 127, 96, 6, 6, 43, 31, 80, 253, 234, 197,
        171, 86, 49, 176, 178, 131, 131, 249, 246, 127, 14, 171, 213, 171, 128, 124,
        32, 0, 0, 199, 52, 231, 199
    ];
    
    
    /* Not byte-to-byte equal, but works, have to check*/
    [Fact]
    public void Serialize()
    {
        SKBitmap bitmap = SKBitmap.Decode(image);
        byte[] bytes = Images.ToTex(bitmap, SquishCompressionType.DXT5, false, true);
        Assert.Equal(tex, bytes);
    }

    [Fact]
    public void Deserialize()
    {
        RTexture texture = new RTexture(tex);
        SKBitmap aimage = texture.getImage();
        byte[] apng = SKImage.FromBitmap(aimage).Encode(SKEncodedImageFormat.Png, 100).ToArray();
        Assert.Equal(image, apng);
    }
}