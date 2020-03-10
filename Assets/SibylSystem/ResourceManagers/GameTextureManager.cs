using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public enum GameTextureType
{
    card_picture = 0,
    card_verticle_drawing = 1,
    card_feature = 3,
}

public class GameTextureManager
{
    static bool bLock = false;

    static Stack<PictureResource> waitLoadStack = new Stack<PictureResource>();

    static Dictionary<ulong, PictureResource> loadedList = new Dictionary<ulong, PictureResource>();

    static Dictionary<ulong, bool> addedMap = new Dictionary<ulong, bool>();

    static readonly HttpDldFile df = new HttpDldFile();

    public class BitmapHelper
    {
        public System.Drawing.Color[,] colors = null;
        public BitmapHelper(string path)
        {
            Bitmap bitmap;
            try
            {
                bitmap = (Bitmap)Image.FromFile(path);
            }
            catch (Exception)
            {
                bitmap = new Bitmap(10, 10);
                for (int i = 0; i < 10; i++)
                {
                    for (int w = 0; w < 10; w++)
                    {
                        bitmap.SetPixel(i, w, System.Drawing.Color.White);
                    }
                }
            }
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            colors = new System.Drawing.Color[bitmap.Width, bitmap.Height];
            for (int counter = 0; counter < rgbValues.Length; counter += 4)
            {
                int i_am = counter / 4;
                colors[i_am % bitmap.Width, i_am / bitmap.Width]
                    =
                    System.Drawing.Color.FromArgb(
                    rgbValues[counter + 3],
                    rgbValues[counter + 2],
                    rgbValues[counter + 1],
                    rgbValues[counter + 0]);
            }
            bitmap.UnlockBits(bmpData);
            bitmap.Dispose();
        }
        public System.Drawing.Color GetPixel(int a, int b)
        {
            return colors[a, b];
        }

    }

    public static void clearUnloaded()
    {
        while (true)
        {
            try
            {
                while (waitLoadStack.Count > 0)
                {
                    var a = waitLoadStack.Pop();
                    addedMap.Remove((UInt64)a.type << 32 | (UInt64)a.code);
                }
                break;
            }
            catch (Exception e)
            {
                Thread.Sleep(10);
                Debug.Log(e);
            }
        }
    }

    public static void clearAll()
    {
        while (true)
        {
            try
            {
                waitLoadStack.Clear();
                loadedList.Clear();
                addedMap.Clear();
                break;
            }
            catch (Exception e)
            {
                Thread.Sleep(10);
                Debug.Log(e);
            }
        }
    }

    private class PictureResource
    {
        public GameTextureType type;
        public long code;
        public bool pCard = false;
        public float k = 1;
        //public bool autoMade = false;
        public byte[] data = null;
        public float[,,] hashed_data = null;
        public Texture2D u_data = null;
        public Texture2D nullReturen = null;
        public PictureResource(GameTextureType t, long c, Texture2D n)
        {
            type = t;
            code = c;
            nullReturen = n;
        }
    }

    private class UIPictureResource
    {
        public string name;
        public Texture2D data = null;
    }

    static BetterList<UIPictureResource> allUI = new BetterList<UIPictureResource>();

    public static Texture2D myBack = null;

    public static Texture2D opBack = null;

    public static Texture2D unknown = null;

    public static Texture2D attack = null;

    public static Texture2D negated = null;

    public static Texture2D bar = null;

    public static Texture2D exBar = null;

    public static Texture2D lp = null;

    public static Texture2D time = null;

    public static Texture2D L = null;

    public static Texture2D R = null;

    public static Texture2D Chain = null;

    public static Texture2D Mask = null;

    public static Texture2D N = null;

    public static Texture2D LINK = null;
    public static Texture2D LINKm = null;


    public static Texture2D nt = null;

    public static Texture2D bp = null;

    public static Texture2D ep = null;

    public static Texture2D mp1 = null;

    public static Texture2D mp2 = null;

    public static Texture2D dp = null;

    public static Texture2D sp = null;



    public static Texture2D phase = null;

    public static bool AutoPicDownload;

    public static Texture2D rs = null;

    public static Texture2D ts = null;

    static void thread_run()
    {
        while (Program.Running)
        {
            try
            {
                Thread.Sleep(50);
                int thu = 0;
                while (waitLoadStack.Count > 0)
                {
                    thu++;
                    if (thu == 10)
                    {
                        Thread.Sleep(50);
                        thu = 0;
                    }
                    if (bLock == false)
                    {
                        PictureResource pic;

                        pic = waitLoadStack.Pop();
                        try
                        {
                            pic.pCard = (YGOSharp.CardsManager.Get((int)pic.code).Type & (int)CardType.Pendulum) > 0;
                        }
                        catch (Exception e)
                        {
                            Debug.Log("e 0" + e.ToString());
                        }
                        switch (pic.type)
                        {
                            case GameTextureType.card_feature:
                            {
                                ProcessingCardFeature(pic);
                                break;
                            }
                            case GameTextureType.card_picture:
                            {
                                ProcessingCardPicture(pic);
                                break;
                            }
                            case GameTextureType.card_verticle_drawing:
                            {
                                ProcessingVerticleDrawing(pic);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("error 1" + e.ToString());
            }
        }
    }

    private static void ProcessingCardFeature(PictureResource pic)
    {
        try
        {
            if (File.Exists("picture/closeup/" + pic.code.ToString() + ".png"))
            {
                string path = "picture/closeup/" + pic.code.ToString() + ".png";
                if (Program.ANDROID_API_N) {
                    BitmapHelper bitmap = new BitmapHelper(path);
                    int left;
                    int right;
                    int up;
                    int down;
                    CutTop(bitmap, out left, out right, out up, out down);
                    up = CutLeft(bitmap, up);
                    down = CutRight(bitmap, down);
                    right = CutButton(bitmap, right);
                    int width = right - left;
                    int height = down - up;
                    pic.hashed_data = new float[width, height, 4];
                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {
                            System.Drawing.Color color = bitmap.GetPixel(left + w, up + h);
                            float a = (float)color.A / 255f;
                            if (w < 40) if (a > (float)w / (float)40) a = (float)w / (float)40;
                            if (w > (width - 40)) if (a > 1f - (float)(w - (width - 40)) / (float)40) a = 1f - (float)(w - (width - 40)) / (float)40;
                            if (h < 40) if (a > (float)h / (float)40) a = (float)h / (float)40;
                            if (h > (height - 40)) if (a > 1f - (float)(h - (height - 40)) / (float)40) a = 1f - (float)(h - (height - 40)) / (float)40;
                            pic.hashed_data[w, height - h - 1, 0] = (float)color.R / 255f;
                            pic.hashed_data[w, height - h - 1, 1] = (float)color.G / 255f;
                            pic.hashed_data[w, height - h - 1, 2] = (float)color.B / 255f;
                            pic.hashed_data[w, height - h - 1, 3] = a;
                        }
                    }
                    caculateK(pic);
                } else {
                    byte[] data;
                    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        file.Seek(0, SeekOrigin.Begin);
                        data = new byte[file.Length];
                        file.Read(data, 0, (int)file.Length);
                    }
                    pic.data = data;
                }

                if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                {
                    loadedList.Add(hashPic(pic.code, pic.type), pic);
                }
            }
            else
            {
                string path = "picture/card/" + pic.code.ToString() + ".png";
                if (!File.Exists(path))
                {
                    path = "picture/card/" + pic.code.ToString() + ".jpg";
                }
                bool Iam8 = false;
                if (!File.Exists(path))
                {
                    Iam8 = true;
                    path = "expansions/pics/" + pic.code.ToString() + ".jpg";
                }
                if (!File.Exists(path))
                {
                    Iam8 = true;
                    path = "pics/" + pic.code.ToString() + ".jpg";
                }
                if (!File.Exists(path))
                {
                    Iam8 = true;
                    path = "picture/cardIn8thEdition/" + pic.code.ToString() + ".jpg";
                }
                if (!File.Exists(path))
                {
                    pic.hashed_data = new float[10, 10, 4];
                    for (int w = 0; w < 10; w++)
                    {
                        for (int h = 0; h < 10; h++)
                        {
                            pic.hashed_data[w, h, 0] = 0;
                            pic.hashed_data[w, h, 1] = 0;
                            pic.hashed_data[w, h, 2] = 0;
                            pic.hashed_data[w, h, 3] = 0;
                        }
                    }
                    if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                    {
                        loadedList.Add(hashPic(pic.code, pic.type), pic);
                    }
                }
                else
                {
                    pic.hashed_data = getCuttedPic(path, pic.pCard,Iam8);
                    int width = pic.hashed_data.GetLength(0);
                    int height = pic.hashed_data.GetLength(1);
                    int size = (int)(height * 0.8);
                    int empWidth = (width - size) / 2;
                    int empHeight = (height - size) / 2;
                    int right = width - empWidth;
                    int buttom = height - empHeight;
                    for (int w = 0; w < width; w++)
                    {
                        for (int h = 0; h < height; h++)
                        {
                            float a = pic.hashed_data[w, h, 3];
                            if (w < empWidth)
                                if (a > ((float)w) / (float)empWidth)
                                    a = ((float)w) / (float)empWidth;
                            if (h < empHeight)
                                if (a > ((float)h) / (float)empHeight)
                                    a = ((float)h) / (float)empHeight;
                            if (w > right)
                                if (a > 1f - ((float)(w - right)) / (float)empWidth)
                                    a = 1f - ((float)(w - right)) / (float)empWidth;
                            if (h > buttom)
                                if (a > 1f - ((float)(h - buttom)) / (float)empHeight)
                                    a = 1f - ((float)(h - buttom)) / (float)empHeight;
                            pic.hashed_data[w, h, 3] = a * 0.7f;
                        }
                    }
                    if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                    {
                        loadedList.Add(hashPic(pic.code, pic.type), pic);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("e 1" + e.ToString());
        }
    }

    private static void caculateK(PictureResource pic)
    {
        //int width = pic.hashed_data.GetLength(0);
        //int height = pic.hashed_data.GetLength(1);
        //int left = 0;
        //int right = width;
        //if (width > height)
        //{
        //    left = (width - height) / 2;
        //    right = width - left;
        //}
        //int all = 0;
        //for (int h = 0; h < height; h++)
        //{
        //    for (int w = left; w < right; w++)
        //    {
        //        if (pic.hashed_data[w, h, 3] > 0.05f)
        //        {
        //            all += 1;
        //        }
        //    }
        //}
        //float result = ((float)all) / (((float)height) * ((float)(height)));
        //pic.k = result + 0.4f;
        //if (pic.k > 1)
        //{
        //    pic.k = 1f;
        //}
        //if (pic.k < 0)
        //{
        //    pic.k = 0.1f;
        //}

        int width = pic.hashed_data.GetLength(0);
        int height = pic.hashed_data.GetLength(1);
        int h = 0;
        for (h = height - 1; h > 0; h--)
        {
            int all = 0;
            for (int w = 0; w < width; w++)
            {
                if (pic.hashed_data[w, h, 3] > 0.05f)
                {
                    all += 1;
                }
            }
            if (all * 5 > width)
            {
                break;
            }
        }
        pic.k = ((float)h) / ((float)height);
        if (pic.k > 1)
        {
            pic.k = 1f;
        }
        if (pic.k < 0)
        {
            pic.k = 0.1f;
        }
    }

    private static float[,,] getCuttedPic(string path, bool pCard, bool EightEdition)
    {
        BitmapHelper bitmap = new BitmapHelper(path);
        int left = 0, top = 0, right = bitmap.colors.GetLength(0), buttom = bitmap.colors.GetLength(1);
        //right is width and buttom is height now
        if (EightEdition)
        {
            if (pCard)
            {
                left = (int)(16f * ((float)right) / 177f);
                right = (int)(162f * ((float)right) / 177f);
                top = (int)(50f * ((float)buttom) / 254f);
                buttom = (int)(158f * ((float)buttom) / 254f);
            }
            else
            {
                left = (int)(26f * ((float)right) / 177f);
                right = (int)(152f * ((float)right) / 177f);
                top = (int)(55f * ((float)buttom) / 254f);
                buttom = (int)(180f * ((float)buttom) / 254f);
            }
        }
        else
        {
            if (pCard)
            {
                left = (int)(25f * ((float)right) / 322f);
                right = (int)(290f * ((float)right) / 322f);
                top = (int)(73f * ((float)buttom) / 402f);
                buttom = (int)(245f * ((float)buttom) / 402f);
            }
            else
            {
                left = (int)(40f * ((float)right) / 322f);
                right = (int)(280f * ((float)right) / 322f);
                top = (int)(75f * ((float)buttom) / 402f);
                buttom = (int)(280f * ((float)buttom) / 402f);
            }
        }
        float[,,] returnValue = new float[right - left, buttom - top, 4];
        for (int w = 0; w < right - left; w++)
        {
            for (int h = 0; h < buttom - top; h++)
            {
                System.Drawing.Color color = bitmap.GetPixel((int)(left + w), (int)(buttom - 1 - h));
                returnValue[w, h, 0] = (float)color.R / 255f;
                returnValue[w, h, 1] = (float)color.G / 255f;
                returnValue[w, h, 2] = (float)color.B / 255f;
                returnValue[w, h, 3] = (float)color.A / 255f;
            }
        }

        return returnValue;
    }

    private static int CutButton(BitmapHelper bitmap, int right)
    {
        for (int w = bitmap.colors.GetLength(0) - 1; w >= 0; w--)
        {
            for (int h = 0; h < bitmap.colors.GetLength(1); h++)
            {
                System.Drawing.Color color = bitmap.GetPixel(w, h);
                if (color.A > 10)
                {
                    right = w;
                    return right;
                }
            }
        }
        return right;
    }

    private static int CutRight(BitmapHelper bitmap, int down)
    {
        for (int h = bitmap.colors.GetLength(1) - 1; h >= 0; h--)
        {
            for (int w = 0; w < bitmap.colors.GetLength(0); w++)
            {
                System.Drawing.Color color = bitmap.GetPixel(w, h);
                if (color.A > 10)
                {
                    down = h;
                    return down;
                }
            }
        }
        return down;
    }

    private static int CutLeft(BitmapHelper bitmap, int up)
    {
        for (int h = 0; h < bitmap.colors.GetLength(1); h++)
        {
            for (int w = 0; w < bitmap.colors.GetLength(0); w++)
            {
                System.Drawing.Color color = bitmap.GetPixel(w, h);
                if (color.A > 10)
                {
                    up = h;
                    return up;
                }
            }
        }
        return up;
    }

    private static void CutTop(BitmapHelper bitmap, out int left, out int right, out int up, out int down)
    {
        ///////切边算法
        left = 0;
        right = bitmap.colors.GetLength(0);
        up = 0;
        down = bitmap.colors.GetLength(1);
        for (int w = 0; w < bitmap.colors.GetLength(0); w++)
        {
            for (int h = 0; h < bitmap.colors.GetLength(1); h++)
            {
                System.Drawing.Color color = bitmap.GetPixel(w, h);
                if (color.A > 10)
                {
                    left = w;
                    return;
                }
            }
        }
    }

    private static void ProcessingVerticleDrawing(PictureResource pic)
    {
        try
        {
            string path = "picture/closeup/" + pic.code.ToString() + ".png";
            if (!File.Exists(path))
            {
                if (Program.ANDROID_API_N) {
                    path = "picture/card/" + pic.code.ToString() + ".png";
                    if (!File.Exists(path))
                    {
                        path = "picture/card/" + pic.code.ToString() + ".jpg";
                    }
                    bool Iam8 = false;
                    if (!File.Exists(path))
                    {
                        Iam8 = true;
                        path = "expansions/pics/" + pic.code.ToString() + ".jpg";
                    }
                    if (!File.Exists(path))
                    {
                        Iam8 = true;
                        path = "pics/" + pic.code.ToString() + ".jpg";
                    }
                    if (!File.Exists(path))
                    {
                        Iam8 = true;
                        path = "picture/cardIn8thEdition/" + pic.code.ToString() + ".jpg";
                    }
                    LoadCloseupFromCardPicture(pic, path, Iam8);
                } else {
                    path = "picture/null.png";
                    byte[] data;
                    using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        file.Seek(0, SeekOrigin.Begin);
                        data = new byte[file.Length];
                        file.Read(data, 0, (int)file.Length);
                    }
                    pic.data = data;

                    if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                    {
                        loadedList.Add(hashPic(pic.code, pic.type), pic);
                    }
                }
            }
            else
            {
                LoadCloseupPicture(pic, path);
            }
        }
        catch (Exception e)
        {
            Debug.Log("e 3" + e.ToString());
        }
    }

    private static void LoadCloseupFromCardPicture(PictureResource pic, string path, bool Iam8)
    {
        try
        {
            if (!File.Exists(path))
            {
                path = "picture/null.png";
            }
            if (!File.Exists(path))
            {
                path = "textures/unknown.jpg";//YGOMobile Paths
            }
            pic.hashed_data = getCuttedPic(path, pic.pCard, Iam8);
            softVtype(pic, 0.5f);
            pic.k = 1;
            if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
            {
                loadedList.Add(hashPic(pic.code, pic.type), pic);
            }
        }
        catch
        {

        }

    }

    private static void LoadCloseupPicture(PictureResource pic, string path)
    {
        try
        {
            if (Program.ANDROID_API_N) {
                BitmapHelper bitmap = new BitmapHelper(path);
                int left;
                int right;
                int up;
                int down;
                CutTop(bitmap, out left, out right, out up, out down);
                up = CutLeft(bitmap, up);
                down = CutRight(bitmap, down);
                right = CutButton(bitmap, right);
                int width = right - left;
                int height = down - up;
                pic.hashed_data = new float[width, height, 4];
                for (int w = 0; w < width; w++)
                {
                    for (int h = 0; h < height; h++)
                    {
                        System.Drawing.Color color = bitmap.GetPixel(left + w, up + h);
                        pic.hashed_data[w, height - h - 1, 0] = (float)color.R / 255f;
                        pic.hashed_data[w, height - h - 1, 1] = (float)color.G / 255f;
                        pic.hashed_data[w, height - h - 1, 2] = (float)color.B / 255f;
                        pic.hashed_data[w, height - h - 1, 3] = (float)color.A / 255f;
                    }
                }
                float wholeUNalpha = 0;
                for (int w = 0; w < width; w++)
                {
                    if (pic.hashed_data[w, 0, 3] > 0.1f)
                    {
                        wholeUNalpha += ((float)Math.Abs(w - width / 2)) / ((float)(width / 2));
                    }
                    if (pic.hashed_data[w, height - 1, 3] > 0.1f)
                    {
                        wholeUNalpha += 1;
                    }
                }
                for (int h = 0; h < height; h++)
                {
                    if (pic.hashed_data[0, h, 3] > 0.1f)
                    {
                        wholeUNalpha += 1;
                    }
                    if (pic.hashed_data[width - 1, h, 3] > 0.1f)
                    {
                        wholeUNalpha += 1;
                    }
                }
                if (wholeUNalpha >= ((width + height) * 0.5f * 0.12f))
                {
                    softVtype(pic, 0.7f);
                }
                caculateK(pic);
            } else {
                byte[] data;
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    file.Seek(0, SeekOrigin.Begin);
                    data = new byte[file.Length];
                    file.Read(data, 0, (int)file.Length);
                }
                pic.data = data;
            }

            if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
            {
                loadedList.Add(hashPic(pic.code, pic.type), pic);
            }
        }
        catch
        {

        }

    }

    private static void softVtype(PictureResource pic, float si)
    {
        int width = pic.hashed_data.GetLength(0);
        int height = pic.hashed_data.GetLength(1);
        int size = (int)(height * si);
        int empWidth = (width - size) / 2;
        int empHeight = (height - size) / 2;
        int right = width - empWidth;
        int buttom = height - empHeight;
        float dui = (float)Math.Sqrt((width / 2) * (width / 2) + (height / 2) * (height / 2));
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                float a = pic.hashed_data[w, h, 3];

                if (h < height / 2)
                {
                    float l = (float)Math.Sqrt((width / 2 - w) * (width / 2 - w) + (height / 2 - h) * (height / 2 - h));
                    l -= width * 0.3f;
                    if (l < 0)
                    {
                        l = 0;
                    }
                    float alpha = 1f - l / (0.6f * (dui - width * 0.3f));
                    if (alpha < 0)
                    {
                        alpha = 0;
                    }
                    if (a > alpha)
                        a = alpha;
                }

                if (w < empWidth)
                    if (a > ((float)w) / (float)empWidth)
                        a = ((float)w) / (float)empWidth;
                if (h < empHeight)
                    if (a > ((float)h) / (float)empHeight)
                        a = ((float)h) / (float)empHeight;
                if (w > right)
                    if (a > 1f - ((float)(w - right)) / (float)empWidth)
                        a = 1f - ((float)(w - right)) / (float)empWidth;
                if (h > buttom)
                    if (a > 1f - ((float)(h - buttom)) / (float)empHeight)
                        a = 1f - ((float)(h - buttom)) / (float)empHeight;
                pic.hashed_data[w, h, 3] = a;
            }
        }
    }

    private static void ProcessingCardPicture(PictureResource pic)
    {
        try
        {
            string path = "picture/card/" + pic.code.ToString() + ".png";
            if (!File.Exists(path))
            {
                path = "picture/card/" + pic.code.ToString() + ".jpg";
            }
            if (!File.Exists(path))
            {
                path = "expansions/pics/" + pic.code.ToString() + ".jpg";
            }
            if (!File.Exists(path))
            {
                path = "pics/" + pic.code.ToString() + ".jpg";
            }
            if (!File.Exists(path))
            {
                path = "picture/cardIn8thEdition/" + pic.code.ToString() + ".jpg";
            }
            if (!File.Exists(path) && pic.code != 0 && AutoPicDownload)
            {
                //YGOMobile (177x254)
                df.Download("http://n.my-card.in/images/ygopro-images-zh-CN/" + pic.code.ToString() + ".jpg", "picture/card/" + pic.code.ToString() + ".jpg");
                path = "picture/card/" + pic.code.ToString() + ".jpg";
            }
            /*if (!File.Exists(path) && pic.code != 0 && AutoPicDownload)
            {
                //先行卡 (177x254)
                df.Download("http://download.ygo2019.xyz/ygopro2-data/expansions/pics/" + pic.code.ToString() + ".jpg", "picture/cardIn8thEdition/" + pic.code.ToString() + ".jpg");
                path = "picture/cardIn8thEdition/" + pic.code.ToString() + ".jpg";
            }*/
            LoadCardPicture(pic, path);
        }
        catch (Exception e)
        {
            Debug.Log("e 2" + e.ToString());
        }
    }

    private static void LoadCardPicture(PictureResource pic, string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                if (pic.code > 0)
                {
                    pic.u_data = unknown;
                }
                else
                {
                    pic.u_data = myBack;
                }
                if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                {
                    loadedList.Add(hashPic(pic.code, pic.type), pic);
                }
            }
            else
            {
                byte[] data;
                using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    file.Seek(0, SeekOrigin.Begin);
                    data = new byte[file.Length];
                    file.Read(data, 0, (int)file.Length);
                }
                pic.data = data;
                if (!loadedList.ContainsKey(hashPic(pic.code, pic.type)))
                {
                    loadedList.Add(hashPic(pic.code, pic.type), pic);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("e 2" + e.ToString());
        }
    }

    private static UInt64 hashPic(long code, GameTextureType type)
    {
        return (((UInt64)type << 32) | ((UInt64)code));
    }

    public static Texture2D get(long code, GameTextureType type, Texture2D nullReturnValue = null)
    {
        try
        {
            PictureResource r;
            if (loadedList.TryGetValue(hashPic(code, type), out r))
            {
                Texture2D re = null;
                if (r.u_data != null)
                {
                    if (r.u_data == myBack)
                    {
                        return nullReturnValue;
                    }
                    else
                    {
                        return r.u_data;
                    }
                }
                if (r.data != null)
                {
                    re = new Texture2D(400, 600);
                    re.LoadImage(r.data);
                    r.u_data = re;
                    return re;
                }
                if (r.hashed_data != null)
                {
                    int width = r.hashed_data.GetLength(0);
                    int height = r.hashed_data.GetLength(1);
                    UnityEngine.Color[] cols = new UnityEngine.Color[width * height];
                    re = new Texture2D(width, height);
                    for (int h = 0; h < height; h++)
                    {
                        for (int w = 0; w < width; w++)
                        {
                            cols[h * width + w] = new UnityEngine.Color(r.hashed_data[w, h, 0], r.hashed_data[w, h, 1], r.hashed_data[w, h, 2], r.hashed_data[w, h, 3]);
                        }
                    }
                    re.SetPixels(0, 0, width, height, cols);
                    re.Apply();
                    r.u_data = re;
                    return re;
                }
            }
            else
            {
                if (!addedMap.ContainsKey(hashPic(code, type)))
                {
                    PictureResource a = new PictureResource(type, code, nullReturnValue);
                    bLock = true;
                    waitLoadStack.Push(a);
                    bLock = false;
                    addedMap.Add((UInt64)type << 32 | (UInt64)code, true);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("BIGERROR1:" + e.ToString());
        }
        return null;
    }

    public static float getK(long code, GameTextureType type)
    {
        float ret = 1;
        PictureResource r;
        if (loadedList.TryGetValue(hashPic(code, type), out r))
        {
            ret = r.k;
        }
        return ret;
    }

    public static bool uiLoaded = false;

    public static Texture2D get(string name)
    {
        if (uiLoaded == false)
        {
            uiLoaded = true;
            FileInfo[] fileInfos = (new DirectoryInfo("textures/ui")).GetFiles();//YGOMobile Paths
            for (int i = 0; i < fileInfos.Length; i++)
            {
                if (fileInfos[i].Name.Length > 4)
                {
                    if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".png")
                    {
                        UIPictureResource r = new UIPictureResource();
                        r.name = fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4);
                        r.data = UIHelper.getTexture2D("textures/ui/" + fileInfos[i].Name);//YGOMobile Paths
                        allUI.Add(r);
                    }
                }
            }
        }
        Texture2D re = null;
        for (int i = 0; i < allUI.size; i++)
        {
            if (allUI[i].name == name)
            {
                re = allUI[i].data;
                break;
            }
        }
        if (re == null)
        {
        }
        return re;
    }

    public static UnityEngine.Color chainColor= UnityEngine.Color.white;

    internal static void initialize()
    {
        attack = UIHelper.getTexture2D("textures/attack.png");//YGOMobile Paths
        myBack = UIHelper.getTexture2D("textures/cover.jpg");//YGOMobile Paths
        opBack = UIHelper.getTexture2D("textures/cover2.jpg");//YGOMobile Paths
        unknown = UIHelper.getTexture2D("textures/unknown.jpg");//YGOMobile Paths
        negated = UIHelper.getTexture2D("textures/negated.png");//YGOMobile Paths
        bar = UIHelper.getTexture2D("textures/duel/healthBar/bg.png");//YGOMobile Paths
        exBar = UIHelper.getTexture2D("textures/duel/healthBar/excited.png");//YGOMobile Paths
        time = UIHelper.getTexture2D("textures/duel/healthBar/t.png");//YGOMobile Paths
        lp = UIHelper.getTexture2D("textures/duel/healthBar/lp.png");//YGOMobile Paths
        L = UIHelper.getTexture2D("textures/duel/L.png");//YGOMobile Paths
        R = UIHelper.getTexture2D("textures/duel/R.png");//YGOMobile Paths
        LINK = UIHelper.getTexture2D("textures/duel/link.png");//YGOMobile Paths
        LINKm = UIHelper.getTexture2D("textures/duel/linkMask.png");//YGOMobile Paths
        Chain = UIHelper.getTexture2D("textures/chain.png");//YGOMobile Paths
        Mask = UIHelper.getTexture2D("textures/mask.png");//YGOMobile Paths


        nt = UIHelper.getTexture2D("textures/duel/phase/nt.png");//YGOMobile Paths
        bp = UIHelper.getTexture2D("textures/duel/phase/bp.png");//YGOMobile Paths
        ep = UIHelper.getTexture2D("textures/duel/phase/ep.png");//YGOMobile Paths
        mp1 = UIHelper.getTexture2D("textures/duel/phase/mp1.png");//YGOMobile Paths
        mp2 = UIHelper.getTexture2D("textures/duel/phase/mp2.png");//YGOMobile Paths
        dp = UIHelper.getTexture2D("textures/duel/phase/dp.png");//YGOMobile Paths
        sp = UIHelper.getTexture2D("textures/duel/phase/sp.png");//YGOMobile Paths

        phase = UIHelper.getTexture2D("textures/duel/phase/phase.png");//YGOMobile Paths

        rs = UIHelper.getTexture2D("textures/duel/phase/rs.png");//YGOMobile Paths
        ts = UIHelper.getTexture2D("textures/duel/phase/ts.png");//YGOMobile Paths

        N = new Texture2D(10,10);
        for (int i = 0; i < 10; i++)
        {
            for (int a = 0; a < 10; a++)
            {
                N.SetPixel(i, a, new UnityEngine.Color(0, 0, 0, 0));
            }
        }
        N.Apply();
        try
        {
            ColorUtility.TryParseHtmlString(File.ReadAllText("textures/duel/chainColor.txt"), out chainColor);//YGOMobile Paths
        }
        catch (Exception)
        {

        }
        Thread main = new Thread(thread_run);
        main.Start();
    }
}
