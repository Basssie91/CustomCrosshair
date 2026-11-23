using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CustomCrosshair
{
    // ------------------------------------------------------------------ instellingen
    public class Settings
    {
        public bool Enabled = true;
        public int Style = 2;
        public Color Color = Color.Lime;
        public int Length = 8;
        public int Thickness = 2;
        public int Gap = 4;
        public int DotSize = 2;
        public bool Outline = true;
        public int Opacity = 100;
        public int OffsetX = 0;
        public int OffsetY = 0;
        public int MenuKey = 0x24;
        public bool ShowMenuOnStart = true;
        public bool StreamProof = false;
        public bool Rgb = false;
        public int RgbSpeed = 4;
        public string MonitorName = "";
        public string UserName = "";
        public bool AutoUpdate = true;
        public string TargetApp = "";
        public int MenuTheme = 4;
        public Color Accent = Color.FromArgb(34, 211, 238);
        public bool StyleV2 = false;
        public bool FivemAutoProfile = true;
        public int OnboardV = 0;
        public bool TourDone = false;
        public string LastSeenVersion = "";
        public string NewsSeenVersion = "";
        public string DiscordToken = "";
        public long DiscordExpires = 0;
        public long DiscordVerifiedUntil = 0;
        public int MenuOpacity = 100;
        public string ImageFile = "";
        public int ImageScale = 100;
        public string Pixels = DefaultPixels();
        public int PixelSize = 2;

        public Settings Clone() { return (Settings)MemberwiseClone(); }

        public static readonly string[] StyleNames = new string[] {
            "Kruis", "Punt", "Kruis + punt", "Cirkel", "Cirkel + punt", "T-vorm",
            "X", "X + punt", "Vierkant", "Vierkant + punt", "Driehoek", "Pijl", "Hoeken", "Boog",
            "Plaatje", "Getekend",
            "Ruit", "Ruit + punt", "Dubbele cirkel", "Cirkel + kruis", "Ster", "Haakjes", "Vier stippen" };
        public const int StyleImage = 14, StylePixels = 15, Grid = 25;

        public static string DefaultPixels()
        {
            char[] a = new string('0', Grid * Grid).ToCharArray();
            int c = Grid / 2;
            a[c * Grid + c] = '1';
            for (int i = 3; i <= 6; i++)
            {
                a[(c - i) * Grid + c] = '1'; a[(c + i) * Grid + c] = '1';
                a[c * Grid + c - i] = '1'; a[c * Grid + c + i] = '1';
            }
            return new string(a);
        }
        public static int MaxStyle { get { return StyleNames.Length - 1; } }

        public string ActiveProfile = "Standaard";

        public static string Dir { get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CustomCrosshair"); } }
        static string FilePath { get { return Path.Combine(Dir, "instellingen.txt"); } }

        public void ResetLook()
        {
            Settings d = new Settings();
            Style = d.Style; Color = d.Color; Length = d.Length;
            Thickness = d.Thickness; Gap = d.Gap; DotSize = d.DotSize; Outline = d.Outline;
            Opacity = d.Opacity; OffsetX = d.OffsetX; OffsetY = d.OffsetY;
            Rgb = d.Rgb; RgbSpeed = d.RgbSpeed;
            ImageScale = d.ImageScale; PixelSize = d.PixelSize;
        }

        public List<string> LookLines()
        {
            List<string> l = new List<string>();
            l.Add("Style=" + Style); l.Add("Color=" + Color.ToArgb()); l.Add("Length=" + Length);
            l.Add("Thickness=" + Thickness); l.Add("Gap=" + Gap); l.Add("DotSize=" + DotSize);
            l.Add("Outline=" + Outline); l.Add("Opacity=" + Opacity);
            l.Add("OffsetX=" + OffsetX); l.Add("OffsetY=" + OffsetY);
            l.Add("Rgb=" + Rgb); l.Add("RgbSpeed=" + RgbSpeed);
            l.Add("ImageFile=" + ImageFile); l.Add("ImageScale=" + ImageScale);
            l.Add("Pixels=" + Pixels); l.Add("PixelSize=" + PixelSize);
            return l;
        }

        public void ApplyLine(string k, string v)
        {
            int n;
            bool ok = int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n);
            switch (k)
            {
                case "Enabled": Enabled = v == "True"; break;
                case "Outline": Outline = v == "True"; break;
                case "ShowMenuOnStart": ShowMenuOnStart = v == "True"; break;
                case "StreamProof": StreamProof = v == "True"; break;
                case "Rgb": Rgb = v == "True"; break;
                case "RgbSpeed": if (ok) RgbSpeed = Math.Max(1, Math.Min(10, n)); break;
                case "MonitorName": MonitorName = v; break;
                case "ActiveProfile": if (v.Length > 0) ActiveProfile = v; break;
                case "Style": if (ok) Style = Math.Max(0, Math.Min(MaxStyle, n)); break;
                case "UserName": UserName = v; break;
                case "AutoUpdate": AutoUpdate = v == "True"; break;
                case "TargetApp": TargetApp = v; break;
                case "MenuTheme": MenuTheme = 4; break;
                case "StyleV2": StyleV2 = v == "True"; break;
                case "FivemAutoProfile": FivemAutoProfile = v == "True"; break;
                case "OnboardV": if (ok) OnboardV = n; break;
                case "TourDone": TourDone = v == "True"; break;
                case "LastSeenVersion": LastSeenVersion = v; break;
                case "NewsSeenVersion": NewsSeenVersion = v; break;
                case "DiscordToken": DiscordToken = v; break;
                case "DiscordExpires": { long lv; if (long.TryParse(v, out lv)) DiscordExpires = lv; } break;
                case "DiscordVerifiedUntil": { long lv; if (long.TryParse(v, out lv)) DiscordVerifiedUntil = lv; } break;
                case "Accent": if (ok) Accent = Color.FromArgb(255, Color.FromArgb(n)); break;
                case "MenuOpacity": if (ok) MenuOpacity = Math.Max(70, Math.Min(100, n)); break;
                case "ImageFile": ImageFile = v; break;
                case "ImageScale": if (ok) ImageScale = Math.Max(10, Math.Min(400, n)); break;
                case "PixelSize": if (ok) PixelSize = Math.Max(1, Math.Min(6, n)); break;
                case "Pixels":
                    if (v.Length == Grid * Grid && v.Trim('0', '1').Length == 0) Pixels = v;
                    break;
                case "Color": if (ok) Color = Color.FromArgb(n); break;
                case "Length": if (ok) Length = n; break;
                case "Thickness": if (ok) Thickness = n; break;
                case "Gap": if (ok) Gap = n; break;
                case "DotSize": if (ok) DotSize = n; break;
                case "Opacity": if (ok) Opacity = n; break;
                case "OffsetX": if (ok) OffsetX = n; break;
                case "OffsetY": if (ok) OffsetY = n; break;
                case "MenuKey": if (ok && n > 0) MenuKey = n; break;
            }
        }

        public static void ReadInto(string path, Settings s)
        {
            foreach (string line in File.ReadAllLines(path))
            {
                int i = line.IndexOf('=');
                if (i > 0) s.ApplyLine(line.Substring(0, i).Trim(), line.Substring(i + 1).Trim());
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                List<string> l = LookLines();
                l.Add("Enabled=" + Enabled); l.Add("MenuKey=" + MenuKey);
                l.Add("ShowMenuOnStart=" + ShowMenuOnStart); l.Add("StreamProof=" + StreamProof);
                l.Add("ActiveProfile=" + ActiveProfile); l.Add("MonitorName=" + MonitorName); l.Add("UserName=" + UserName); l.Add("AutoUpdate=" + AutoUpdate); l.Add("TargetApp=" + TargetApp);
                l.Add("MenuTheme=" + MenuTheme); l.Add("StyleV2=" + StyleV2); l.Add("FivemAutoProfile=" + FivemAutoProfile);
                l.Add("OnboardV=" + OnboardV); l.Add("TourDone=" + TourDone); l.Add("LastSeenVersion=" + LastSeenVersion); l.Add("NewsSeenVersion=" + NewsSeenVersion);
                l.Add("DiscordToken=" + DiscordToken); l.Add("DiscordExpires=" + DiscordExpires); l.Add("DiscordVerifiedUntil=" + DiscordVerifiedUntil); l.Add("Accent=" + Accent.ToArgb()); l.Add("MenuOpacity=" + MenuOpacity);
                File.WriteAllLines(FilePath, l.ToArray());
            }
            catch { }
        }

        public static Settings Load()
        {
            Settings s = new Settings();
            try { if (File.Exists(FilePath)) ReadInto(FilePath, s); } catch { }
            return s;
        }
    }

    // ------------------------------------------------------------------ profielen
    public static class Profiles
    {
        public static string Dir { get { return Path.Combine(Settings.Dir, "profielen"); } }
        static string PathFor(string n) { return Path.Combine(Dir, n + ".txt"); }

        public static List<string> Names()
        {
            List<string> l = new List<string>();
            try
            {
                if (Directory.Exists(Dir))
                    foreach (string f in Directory.GetFiles(Dir, "*.txt")) l.Add(Path.GetFileNameWithoutExtension(f));
            }
            catch { }
            l.Sort(StringComparer.OrdinalIgnoreCase);
            return l;
        }

        public static bool Exists(string n) { return File.Exists(PathFor(n)); }

        public static void Save(string n, Settings s)
        {
            try { Directory.CreateDirectory(Dir); File.WriteAllLines(PathFor(n), s.LookLines().ToArray()); } catch { }
        }

        public static void Load(string n, Settings s)
        {
            try { Settings.ReadInto(PathFor(n), s); } catch { }
        }

        public static bool Matches(string n, Settings s)
        {
            try
            {
                if (!Exists(n)) return false;
                return string.Join("\n", File.ReadAllLines(PathFor(n))).Trim() == string.Join("\n", s.LookLines().ToArray()).Trim();
            }
            catch { return false; }
        }

        public static void Delete(string n)
        {
            try { File.Delete(PathFor(n)); } catch { }
        }

        public static string Clean(string n)
        {
            if (n == null) return "";
            foreach (char c in Path.GetInvalidFileNameChars()) n = n.Replace(c.ToString(), "");
            n = n.Trim();
            if (n.Length > 24) n = n.Substring(0, 24).Trim();
            return n;
        }
    }

    // ------------------------------------------------------------------ crosshair-code
    public static class ShareCode
    {
        static int Clamp(int v, int min, int max) { return Math.Max(min, Math.Min(max, v)); }

        static byte Check(byte[] b)
        {
            int sum = 0;
            for (int i = 0; i < 14; i++) sum += b[i] * (i + 1);
            return (byte)((sum ^ 0x5A) & 0xFF);
        }

        public static string Make(Settings s)
        {
            byte[] b = new byte[15];
            b[0] = 1;
            b[1] = (byte)Clamp(s.Style, 0, Settings.MaxStyle);
            b[2] = s.Color.R; b[3] = s.Color.G; b[4] = s.Color.B;
            b[5] = (byte)Clamp(s.Length, 1, 40);
            b[6] = (byte)Clamp(s.Thickness, 1, 10);
            b[7] = (byte)Clamp(s.Gap, 0, 30);
            b[8] = (byte)Clamp(s.DotSize, 1, 12);
            b[9] = (byte)((s.Outline ? 1 : 0) | (s.Rgb ? 2 : 0));
            b[10] = (byte)Clamp(s.Opacity, 10, 100);
            b[11] = (byte)(Clamp(s.OffsetX, -60, 60) + 128);
            b[12] = (byte)(Clamp(s.OffsetY, -60, 60) + 128);
            b[13] = (byte)Clamp(s.RgbSpeed, 1, 10);
            if (s.Style == Settings.StylePixels)
            {
                byte[] big = new byte[15 + 1 + 79];
                Array.Copy(b, big, 15);
                big[0] = 2;
                big[15] = (byte)Clamp(s.PixelSize, 1, 6);
                for (int i = 0; i < Settings.Grid * Settings.Grid && i < s.Pixels.Length; i++)
                    if (s.Pixels[i] == '1') big[16 + i / 8] |= (byte)(1 << (i % 8));
                b = big;
            }
            b[14] = Check(b);
            return "BC-" + Convert.ToBase64String(b).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        public static bool Apply(string code, Settings s)
        {
            try
            {
                if (code == null) return false;
                code = code.Trim();
                if (!code.StartsWith("BC-", StringComparison.OrdinalIgnoreCase)) return false;
                string b64 = code.Substring(3).Replace('-', '+').Replace('_', '/');
                while (b64.Length % 4 != 0) b64 += "=";
                byte[] b = Convert.FromBase64String(b64);
                if (b.Length < 15 || (b[0] != 1 && b[0] != 2) || b[14] != Check(b)) return false;
                if (b[0] == 2 && b.Length < 15 + 1 + 79) return false;
                s.Style = Clamp(b[1], 0, Settings.MaxStyle);
                s.Color = Color.FromArgb(b[2], b[3], b[4]);
                s.Length = Clamp(b[5], 1, 40);
                s.Thickness = Clamp(b[6], 1, 10);
                s.Gap = Clamp(b[7], 0, 30);
                s.DotSize = Clamp(b[8], 1, 12);
                s.Outline = (b[9] & 1) != 0;
                s.Rgb = (b[9] & 2) != 0;
                s.Opacity = Clamp(b[10], 10, 100);
                s.OffsetX = Clamp(b[11] - 128, -60, 60);
                s.OffsetY = Clamp(b[12] - 128, -60, 60);
                s.RgbSpeed = Clamp(b[13], 1, 10);
                if (b[0] == 2)
                {
                    s.PixelSize = Clamp(b[15], 1, 6);
                    char[] px = new char[Settings.Grid * Settings.Grid];
                    for (int i = 0; i < px.Length; i++) px[i] = (b[16 + i / 8] & (1 << (i % 8))) != 0 ? '1' : '0';
                    s.Pixels = new string(px);
                }
                return true;
            }
            catch { return false; }
        }
    }

    // ------------------------------------------------------------------ icoon
    public static class AppIcon
    {
        public static void Draw(Graphics g, int size)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            float s = size;
            RectangleF r = new RectangleF(s * 0.02f, s * 0.02f, s * 0.96f, s * 0.96f);
            using (GraphicsPath p = Theme.Round(r, s * 0.22f))
            using (LinearGradientBrush b = new LinearGradientBrush(new RectangleF(0, 0, s, s), Color.FromArgb(26, 28, 34), Color.FromArgb(8, 8, 10), 90f))
            {
                g.FillPath(b, p);
                if (size >= 48)
                    using (Pen pe = new Pen(Color.FromArgb(48, 52, 62), Math.Max(1f, s / 96f))) g.DrawPath(pe, p);
            }
            float cx = s / 2f, cy = s / 2f, R = s * (size <= 24 ? 0.36f : 0.33f);
            PointF[] hex = new PointF[6];
            for (int k = 0; k < 6; k++)
            {
                double ang = Math.PI / 6 + k * Math.PI / 3;
                hex[k] = new PointF(cx + (float)(R * Math.Cos(ang)), cy + (float)(R * Math.Sin(ang)));
            }
            float w = Math.Max(1.6f, s * (size <= 24 ? 0.09f : 0.06f));
            using (Pen p = new Pen(Color.FromArgb(34, 211, 238), w))
            {
                p.LineJoin = LineJoin.Round;
                g.DrawPolygon(p, hex);
            }
            float gap = s * 0.09f, L = s * 0.13f;
            float tw = Math.Max(1.4f, s * (size <= 24 ? 0.085f : 0.05f));
            if (size <= 24) { gap = s * 0.08f; L = s * 0.14f; }
            using (Pen p = new Pen(Color.White, tw))
            {
                g.DrawLine(p, cx + gap, cy, cx + gap + L, cy);
                g.DrawLine(p, cx - gap, cy, cx - gap - L, cy);
                g.DrawLine(p, cx, cy + gap, cx, cy + gap + L);
                g.DrawLine(p, cx, cy - gap, cx, cy - gap - L);
            }
        }

        public static Bitmap MakeBitmap(int size)
        {
            Bitmap bmp = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bmp)) { g.Clear(Color.Transparent); Draw(g, size); }
            return bmp;
        }

        public static Icon MakeIcon(int size)
        {
            using (Bitmap b = MakeBitmap(size)) return Icon.FromHandle(b.GetHicon());
        }

        public static void WriteIco(string path)
        {
            int[] sizes = new int[] { 16, 20, 24, 32, 40, 48, 64, 128, 256 };
            List<byte[]> data = new List<byte[]>();
            foreach (int sz in sizes)
                using (Bitmap b = MakeBitmap(sz))
                using (MemoryStream ms = new MemoryStream()) { b.Save(ms, ImageFormat.Png); data.Add(ms.ToArray()); }
            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter w = new BinaryWriter(fs))
            {
                w.Write((ushort)0); w.Write((ushort)1); w.Write((ushort)sizes.Length);
                int offset = 6 + 16 * sizes.Length;
                for (int k = 0; k < sizes.Length; k++)
                {
                    w.Write((byte)(sizes[k] >= 256 ? 0 : sizes[k]));
                    w.Write((byte)(sizes[k] >= 256 ? 0 : sizes[k]));
                    w.Write((byte)0); w.Write((byte)0);
                    w.Write((ushort)1); w.Write((ushort)32);
                    w.Write((uint)data[k].Length); w.Write((uint)offset);
                    offset += data[k].Length;
                }
                foreach (byte[] d in data) w.Write(d);
            }
        }
    }

    // ------------------------------------------------------------------ crosshair tekenen
    public static class Crosshair
    {
        public static float Hue = 0f;

        public static Color ColorFor(Settings s)
        {
            if (!s.Rgb) return s.Color;
            float h = (Hue % 360f) / 60f;
            int i = (int)Math.Floor(h) % 6;
            float f = h - (float)Math.Floor(h);
            int q = (int)(255 * (1 - f)), u = (int)(255 * f);
            switch (i)
            {
                case 0: return Color.FromArgb(255, u, 0);
                case 1: return Color.FromArgb(q, 255, 0);
                case 2: return Color.FromArgb(0, 255, u);
                case 3: return Color.FromArgb(0, q, 255);
                case 4: return Color.FromArgb(u, 0, 255);
                default: return Color.FromArgb(255, 0, q);
            }
        }

        static Bitmap cachedImg; static string cachedName;

        public static string ImageDir { get { return Path.Combine(Settings.Dir, "plaatjes"); } }

        static Bitmap Prepare(Image src)
        {
            Bitmap tmp = new Bitmap(src);
            Bitmap b = new Bitmap(tmp.Width, tmp.Height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < tmp.Height; y++)
                for (int x = 0; x < tmp.Width; x++)
                {
                    Color c = tmp.GetPixel(x, y);
                    if (c.A >= 128)
                    {
                        if (c.R == 255 && c.G == 0 && c.B == 255) c = Color.FromArgb(254, 0, 255);
                        b.SetPixel(x, y, Color.FromArgb(255, c.R, c.G, c.B));
                    }
                    else b.SetPixel(x, y, Color.Transparent);
                }
            tmp.Dispose();
            return b;
        }

        public static string ImportImage(string path)
        {
            try
            {
                Directory.CreateDirectory(ImageDir);
                Bitmap bmp;
                using (FileStream fs = File.OpenRead(path))
                using (Image im = Image.FromStream(fs))
                {
                    int w = im.Width, h = im.Height;
                    int max = Math.Max(w, h);
                    if (max > 256) { w = Math.Max(1, w * 256 / max); h = Math.Max(1, h * 256 / max); }
                    bmp = new Bitmap(im, w, h);
                }
                string name = Path.GetFileNameWithoutExtension(path);
                foreach (char c in Path.GetInvalidFileNameChars()) name = name.Replace(c.ToString(), "");
                if (name.Length == 0) name = "plaatje";
                name = name + ".png";
                bmp.Save(Path.Combine(ImageDir, name), ImageFormat.Png);
                bmp.Dispose();
                cachedName = null;
                return name;
            }
            catch { return null; }
        }

        static Bitmap GetImage(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            if (name == cachedName) return cachedImg;
            cachedName = name;
            if (cachedImg != null) { cachedImg.Dispose(); cachedImg = null; }
            try
            {
                string p = Path.Combine(ImageDir, name);
                if (File.Exists(p))
                    using (FileStream fs = File.OpenRead(p))
                    using (Image im = Image.FromStream(fs))
                        cachedImg = Prepare(im);
            }
            catch { cachedImg = null; }
            return cachedImg;
        }

        // Hoe groot het overlay-venster moet zijn (klein = minder werk voor Windows)
        public static int NeededSize(Settings s)
        {
            int size = 400;
            if (s.Style == Settings.StyleImage)
            {
                Bitmap img = GetImage(s.ImageFile);
                if (img != null)
                {
                    int need = (int)(Math.Max(img.Width, img.Height) * Math.Max(10, s.ImageScale) / 100f) + 160;
                    size = Math.Max(size, need);
                }
            }
            if (size % 2 == 1) size++;
            return size;
        }

        public static void Draw(Graphics g, Settings s, int cx, int cy)
        {
            g.SmoothingMode = SmoothingMode.None;
            if (s.Style == Settings.StyleImage)
            {
                Bitmap img = GetImage(s.ImageFile);
                if (img == null)
                {
                    using (SolidBrush fb = new SolidBrush(Color.White)) g.FillRectangle(fb, cx - 1, cy - 1, 3, 3);
                    return;
                }
                float sc = Math.Max(10, s.ImageScale) / 100f;
                float w = img.Width * sc, h = img.Height * sc;
                GraphicsState gs = g.Save();
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.Half;
                g.DrawImage(img, new RectangleF((float)Math.Round(cx - w / 2), (float)Math.Round(cy - h / 2), w, h));
                g.Restore(gs);
                return;
            }
            Color c = ColorFor(s);
            if (c.R == 255 && c.G == 0 && c.B == 255) c = Color.FromArgb(254, 0, 255);
            c = Color.FromArgb(255, c.R, c.G, c.B);
            if (s.Outline) Shape(g, s, cx, cy, Color.FromArgb(1, 1, 1), 1);
            Shape(g, s, cx, cy, c, 0);
        }

        static void Box(Graphics g, Brush b, int x, int y, int w, int h, int grow)
        {
            g.FillRectangle(b, x - grow, y - grow, w + grow * 2, h + grow * 2);
        }

        static Pen MkPen(Color c, float w)
        {
            Pen p = new Pen(c, w);
            p.StartCap = LineCap.Square; p.EndCap = LineCap.Square; p.LineJoin = LineJoin.Miter;
            return p;
        }

        static void Shape(Graphics g, Settings s, int cx, int cy, Color color, int grow)
        {
            int t = Math.Max(1, s.Thickness);
            int len = Math.Max(1, s.Length);
            int gap = Math.Max(0, s.Gap);
            int h = t / 2;
            int st = s.Style;
            bool cross = st == 0 || st == 2 || st == 5 || st == 19 || st == 20;
            bool dot = st == 1 || st == 2 || st == 4 || st == 7 || st == 9 || st == 13 || st == 17;
            bool diamond = st == 16 || st == 17;
            bool dblCircle = st == 18;
            bool brackets = st == 21;
            bool fourDots = st == 22;
            bool arc = st == 13;
            bool pix = st == Settings.StylePixels;
            bool circle = st == 3 || st == 4 || st == 18 || st == 19;
            bool noTop = st == 5;
            bool xs = st == 6 || st == 7 || st == 20;
            bool square = st == 8 || st == 9;
            bool tri = st == 10;
            bool chev = st == 11;
            bool corners = st == 12;
            float pw = t + grow * 2;
            int r = Math.Max(3, gap + len / 2);

            using (SolidBrush b = new SolidBrush(color))
            {
                if (pix && s.Pixels != null)
                {
                    int N = Settings.Grid, half = N / 2, ps = Math.Max(1, s.PixelSize);
                    for (int i = 0; i < N * N && i < s.Pixels.Length; i++)
                        if (s.Pixels[i] == '1')
                            Box(g, b, cx + (i % N - half) * ps - ps / 2, cy + (i / N - half) * ps - ps / 2, ps, ps, grow);
                }
                if (cross)
                {
                    Box(g, b, cx + gap, cy - h, len, t, grow);
                    Box(g, b, cx - gap - len, cy - h, len, t, grow);
                    Box(g, b, cx - h, cy + gap, t, len, grow);
                    if (!noTop) Box(g, b, cx - h, cy - gap - len, t, len, grow);
                }
            }
            using (Pen p = MkPen(color, pw))
            {
                if (circle) g.DrawEllipse(p, cx - r, cy - r, r * 2, r * 2);
                if (dblCircle)
                {
                    int r2 = r + t * 2 + 3;
                    g.DrawEllipse(p, cx - r2, cy - r2, r2 * 2, r2 * 2);
                }
                if (diamond)
                    g.DrawPolygon(p, new PointF[] { new PointF(cx, cy - r), new PointF(cx + r, cy), new PointF(cx, cy + r), new PointF(cx - r, cy) });
                if (brackets)
                {
                    int arm = Math.Max(2, len / 3);
                    g.DrawLines(p, new PointF[] { new PointF(cx - r + arm, cy - r), new PointF(cx - r, cy - r), new PointF(cx - r, cy + r), new PointF(cx - r + arm, cy + r) });
                    g.DrawLines(p, new PointF[] { new PointF(cx + r - arm, cy - r), new PointF(cx + r, cy - r), new PointF(cx + r, cy + r), new PointF(cx + r - arm, cy + r) });
                }
                if (xs)
                {
                    float d0 = gap * 0.7071f, d1 = (gap + len) * 0.7071f;
                    g.DrawLine(p, cx + d0, cy + d0, cx + d1, cy + d1);
                    g.DrawLine(p, cx - d0, cy - d0, cx - d1, cy - d1);
                    g.DrawLine(p, cx + d0, cy - d0, cx + d1, cy - d1);
                    g.DrawLine(p, cx - d0, cy + d0, cx - d1, cy + d1);
                }
                if (square) g.DrawRectangle(p, cx - r, cy - r, r * 2, r * 2);
                if (tri)
                {
                    PointF[] pts = new PointF[] {
                        new PointF(cx, cy - r), new PointF(cx + r * 0.866f, cy + r * 0.5f), new PointF(cx - r * 0.866f, cy + r * 0.5f) };
                    g.DrawPolygon(p, pts);
                }
                if (chev)
                {
                    PointF[] pts = new PointF[] {
                        new PointF(cx - len, cy + gap + len), new PointF(cx, cy + gap), new PointF(cx + len, cy + gap + len) };
                    g.DrawLines(p, pts);
                }
                if (arc)
                {
                    int ly = cy + Math.Max(2, gap + 2);
                    int ra = Math.Max(4, len);
                    g.DrawLine(p, cx - ra - len, ly, cx - ra, ly);
                    g.DrawLine(p, cx + ra, ly, cx + ra + len, ly);
                    g.DrawArc(p, cx - ra, ly - ra, ra * 2, ra * 2, 0, 180);
                }
                if (corners)
                {
                    int cr = gap + 4;
                    int arm = Math.Max(2, len / 2);
                    for (int sx = -1; sx <= 1; sx += 2)
                        for (int sy = -1; sy <= 1; sy += 2)
                        {
                            PointF[] pts = new PointF[] {
                                new PointF(cx + sx * (cr - arm), cy + sy * cr),
                                new PointF(cx + sx * cr, cy + sy * cr),
                                new PointF(cx + sx * cr, cy + sy * (cr - arm)) };
                            g.DrawLines(p, pts);
                        }
                }
            }
            if (fourDots)
            {
                int fd = Math.Max(2, s.DotSize);
                int off = gap + len / 2;
                using (SolidBrush b4 = new SolidBrush(color))
                {
                    Box(g, b4, cx + off - fd / 2, cy - fd / 2, fd, fd, grow);
                    Box(g, b4, cx - off - fd / 2, cy - fd / 2, fd, fd, grow);
                    Box(g, b4, cx - fd / 2, cy + off - fd / 2, fd, fd, grow);
                    Box(g, b4, cx - fd / 2, cy - off - fd / 2, fd, fd, grow);
                }
            }
            if (dot)
            {
                int d = Math.Max(1, s.DotSize);
                using (SolidBrush b = new SolidBrush(color)) Box(g, b, cx - d / 2, cy - d / 2, d, d, grow);
            }
        }
    }

    // ------------------------------------------------------------------ overlay
    public class Overlay : Form
    {
        Settings s;
        static readonly Color Key = Color.FromArgb(255, 0, 255);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr after, int x, int y, int cx, int cy, uint flags);

        public Overlay(Settings settings)
        {
            s = settings;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            Bounds = new Rectangle(0, 0, 400, 400);
            BackColor = Key;
            TransparencyKey = Key;
            DoubleBuffered = true;
            Timer t = new Timer();
            t.Interval = 2500;
            t.Tick += delegate { KeepOnTop(); };
            t.Start();
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x80000 | 0x20 | 0x80 | 0x8000000 | 0x8;
                return cp;
            }
        }

        void KeepOnTop()
        {
            if (Visible && IsHandleCreated)
                SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0, 0x0001 | 0x0002 | 0x0010 | 0x0200 | 0x0400);
        }

        public void InvalidateCrosshair()
        {
            if (!Visible) return;
            Invalidate();
        }

        public bool AppAllowed = true;

        public void ApplySettings()
        {
            Rectangle scr = Program.TargetScreen(s).Bounds;
            int size = Crosshair.NeededSize(s);
            Rectangle target = new Rectangle(scr.X + scr.Width / 2 - size / 2, scr.Y + scr.Height / 2 - size / 2, size, size);
            if (Bounds != target) Bounds = target;
            double op = Math.Max(10, Math.Min(100, s.Opacity)) / 100.0;
            if (Math.Abs(Opacity - op) > 0.001) Opacity = op;
            bool show = s.Enabled && AppAllowed;
            if (show && !Visible) Show();
            if (!show && Visible) Hide();
            Invalidate();
            KeepOnTop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Crosshair.Draw(e.Graphics, s, ClientSize.Width / 2 + s.OffsetX, ClientSize.Height / 2 + s.OffsetY);
        }
    }

    // ------------------------------------------------------------------ thema
    public static class Theme
    {
        public static Color Window = Color.FromArgb(17, 17, 20);
        public static Color Sidebar = Color.FromArgb(12, 12, 14);
        public static Color Card = Color.FromArgb(23, 23, 27);
        public static Color Border = Color.FromArgb(38, 38, 44);
        public static Color Hover = Color.FromArgb(30, 30, 36);
        public static Color Selected = Color.FromArgb(36, 36, 42);
        public static readonly Color Text = Color.FromArgb(228, 228, 232);
        public static readonly Color Dim = Color.FromArgb(128, 128, 138);
        public static readonly Color Faint = Color.FromArgb(78, 78, 88);
        public static readonly Color Danger = Color.FromArgb(235, 95, 95);
        public static Color Chip = Color.FromArgb(32, 32, 38);
        public static Color ChipHover = Color.FromArgb(40, 40, 47);
        public static Color Track = Color.FromArgb(46, 46, 52);

        public static readonly Font Normal = new Font("Segoe UI", 9.5f);
        public static readonly Font Bold = new Font("Segoe UI Semibold", 9.5f);
        public static readonly Font Small = new Font("Segoe UI", 7.5f, FontStyle.Bold);
        public static readonly Font Tiny = new Font("Segoe UI", 8f);
        public static readonly Font Title = new Font("Segoe UI Semibold", 11f);
        public static readonly Font PageFont = new Font("Segoe UI Semibold", 18f);
        public static readonly Font IconFont = new Font("Segoe MDL2 Assets", 10f);
        public static readonly Font Heavy = new Font("Segoe UI Black", 13f, FontStyle.Italic);
        public static readonly Font HeavySmall = new Font("Segoe UI Black", 9.5f, FontStyle.Italic);
        public static readonly Font Spaced = new Font("Segoe UI", 7.5f, FontStyle.Bold);

        public static int Style = 0;   // 0 = Clean, 1 = Lounge
        public static Color Accent = Color.FromArgb(225, 225, 230);
        public static Color AccentText = Color.FromArgb(18, 18, 22);
        public static readonly string[] StyleNames = new string[] { "Clean", "Lounge", "Midnight", "OLED", "Glass" };

        // window, sidebar, card, border, hover, selected, chip, chipHover, track
        public static Color[] PaletteFor(int style)
        {
            int[][] v;
            if (style == 2) v = new int[][] { new int[]{13,17,28}, new int[]{9,12,21}, new int[]{19,24,38}, new int[]{33,41,62}, new int[]{25,31,48}, new int[]{31,39,60}, new int[]{30,37,56}, new int[]{39,48,72}, new int[]{42,51,76} };
            else if (style == 4) v = new int[][] { new int[]{27,31,41}, new int[]{22,25,34}, new int[]{40,46,60}, new int[]{60,68,86}, new int[]{49,56,72}, new int[]{57,65,84}, new int[]{47,54,70}, new int[]{57,65,84}, new int[]{66,74,94} };
            else if (style == 3) v = new int[][] { new int[]{0,0,0}, new int[]{0,0,0}, new int[]{11,11,13}, new int[]{30,30,34}, new int[]{20,20,24}, new int[]{26,26,30}, new int[]{24,24,28}, new int[]{34,34,40}, new int[]{38,38,44} };
            else v = new int[][] { new int[]{17,17,20}, new int[]{12,12,14}, new int[]{23,23,27}, new int[]{38,38,44}, new int[]{30,30,36}, new int[]{36,36,42}, new int[]{32,32,38}, new int[]{40,40,47}, new int[]{46,46,52} };
            Color[] c = new Color[v.Length];
            for (int i = 0; i < v.Length; i++) c[i] = Color.FromArgb(v[i][0], v[i][1], v[i][2]);
            return c;
        }

        public static void Apply(int style, Color accent)
        {
            style = 4;   // Glass is de enige stijl
            Style = style;
            Color[] pal = PaletteFor(style);
            Window = pal[0]; Sidebar = pal[1]; Card = pal[2]; Border = pal[3]; Hover = pal[4];
            Selected = pal[5]; Chip = pal[6]; ChipHover = pal[7]; Track = pal[8];
            Accent = Color.FromArgb(255, accent.R, accent.G, accent.B);
            int lum = (accent.R * 299 + accent.G * 587 + accent.B * 114) / 1000;
            AccentText = lum > 150 ? Color.FromArgb(18, 18, 22) : Color.White;
        }

        public static bool Lounge { get { return Style == 1; } }
        public static bool Glass { get { return Style == 4; } }
        public static Font Header { get { return Lounge ? HeavySmall : Bold; } }
        public static string Up(string s) { return Lounge ? s.ToUpper() : s; }

        public static string Caps(string s)
        {
            return string.Join("\u2009", Array.ConvertAll(s.ToUpper().ToCharArray(), c => c.ToString()));
        }

        public static void Bar(Graphics g, Color c, int x, int y, int w)
        {
            using (SolidBrush b = new SolidBrush(c)) g.FillRectangle(b, x, y, w, 3);
        }
        public static readonly bool HasIcons = IconFont.Name == "Segoe MDL2 Assets";

        public static GraphicsPath Round(RectangleF r, float rad)
        {
            GraphicsPath p = new GraphicsPath();
            float d = rad * 2;
            if (d > r.Height) d = r.Height;
            if (d > r.Width) d = r.Width;
            if (d < 1) { p.AddRectangle(r); return p; }
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
            return p;
        }

        public static void FillRound(Graphics g, Color c, RectangleF r, float rad)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(c))
            using (GraphicsPath p = Round(r, rad))
                g.FillPath(b, p);
        }

        public static void DrawRound(Graphics g, Color c, RectangleF r, float rad)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen pen = new Pen(c, 1f))
            using (GraphicsPath p = Round(r, rad))
                g.DrawPath(pen, p);
        }

        public static void Label(Graphics g, string t, Font f, Color c, Rectangle r, TextFormatFlags extra)
        {
            TextRenderer.DrawText(g, t, f, r, c, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | extra);
        }

        public static void Glyph(Graphics g, string glyph, Color c, Rectangle r)
        {
            if (glyph == null) return;
            if (glyph == "X") { CrossIcon(g, c, r, 1.5f); return; }
            if (!HasIcons) return;
            TextRenderer.DrawText(g, glyph, IconFont, r, c, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        public static void CrossIcon(Graphics g, Color c, Rectangle r, float w)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            float cx = r.X + r.Width / 2f, cy = r.Y + r.Height / 2f;
            float rad = Math.Min(r.Width, r.Height) / 2f - 3f;
            float tick = rad * 0.55f;
            using (Pen p = new Pen(c, w))
            {
                g.DrawEllipse(p, cx - rad, cy - rad, rad * 2, rad * 2);
                g.DrawLine(p, cx, cy - rad - 2, cx, cy - rad + tick);
                g.DrawLine(p, cx, cy + rad + 2, cx, cy + rad - tick);
                g.DrawLine(p, cx - rad - 2, cy, cx - rad + tick, cy);
                g.DrawLine(p, cx + rad + 2, cy, cx + rad - tick, cy);
            }
            using (SolidBrush b = new SolidBrush(c))
                g.FillEllipse(b, cx - w, cy - w, w * 2, w * 2);
        }

        public static string KeyName(int vk)
        {
            string n = ((Keys)vk).ToString();
            if (n.Length == 2 && n[0] == 'D' && char.IsDigit(n[1])) n = n.Substring(1);
            if (n == "Next") n = "PageDown";
            if (n == "Prior") n = "PageUp";
            return n.ToUpper();
        }
    }

    // ------------------------------------------------------------------ eigen besturingselementen
    public class Ctl : Control
    {
        protected bool Hover;
        public Ctl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.StandardDoubleClick, false);
            BackColor = Theme.Card;
            Font = Theme.Normal;
        }
        protected override void OnMouseEnter(EventArgs e) { Hover = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { Hover = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnPaintBackground(PaintEventArgs e) { e.Graphics.Clear(BackColor); }
    }

    public class Row : Ctl
    {
        protected string Glyph;
        protected string Caption;
        public Row(string glyph, string caption, int height) { Glyph = glyph; Caption = caption; Height = height; }
        protected void PaintCaption(Graphics g, int x, int top, int h, Color c, int width)
        {
            if (Theme.Lounge)
            {
                Theme.Glyph(g, Glyph, c, new Rectangle(x, top, 18, h));
                Theme.Label(g, Caption, Theme.Normal, c, new Rectangle(x + 26, top, Math.Max(10, width - 26), h), TextFormatFlags.Left);
            }
            else Theme.Label(g, Caption, Theme.Normal, c, new Rectangle(x, top, Math.Max(10, width), h), TextFormatFlags.Left);
        }
    }

    public class ToggleRow : Row
    {
        bool on; Action<bool> cb;
        public ToggleRow(string glyph, string caption, bool value, Action<bool> cb) : base(glyph, caption, 30)
        { on = value; this.cb = cb; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Height, on ? Theme.Text : Theme.Dim, Width - 50);
            RectangleF r = new RectangleF(Width - 36, (Height - 18) / 2f, 34, 18);
            Theme.FillRound(g, on ? Theme.Accent : Theme.Track, r, 9);
            float k = 12;
            float kx = on ? r.Right - k - 3 : r.X + 3;
            using (SolidBrush b = new SolidBrush(on ? Theme.AccentText : Color.FromArgb(95, 95, 104)))
                g.FillEllipse(b, kx, r.Y + 3, k, k);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { on = !on; Invalidate(); cb(on); }
            base.OnMouseClick(e);
        }
    }

    public class SliderRow : Row
    {
        int min, max, val; bool drag;
        Action<int> cb; Func<int, string> fmt;
        public SliderRow(string glyph, string caption, int min, int max, int value, Func<int, string> fmt, Action<int> cb)
            : base(glyph, caption, 46)
        {
            this.min = min; this.max = max; this.fmt = fmt; this.cb = cb;
            val = Math.Max(min, Math.Min(max, value));
            Cursor = Cursors.Hand;
        }
        Rectangle Track { get { return new Rectangle(8, 32, Width - 16, 4); } }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 2, 22, Theme.Text, Width - 90);
            Theme.Label(g, fmt(val), Theme.Normal, Theme.Dim, new Rectangle(Width - 90, 2, 90, 22), TextFormatFlags.Right);
            Rectangle t = Track;
            Theme.FillRound(g, Theme.Track, t, 2);
            float f = (val - min) / (float)(max - min);
            float x = t.X + f * t.Width;
            Theme.FillRound(g, Theme.Accent, new RectangleF(t.X, t.Y, x - t.X, t.Height), 2);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush sh = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
                g.FillEllipse(sh, x - 8, t.Y + 2 - 7, 16, 16);
            using (SolidBrush b = new SolidBrush(drag ? Color.White : Color.FromArgb(240, 240, 244)))
                g.FillEllipse(b, x - 7, t.Y + 2 - 7, 14, 14);
        }
        void SetFromMouse(int mx)
        {
            Rectangle t = Track;
            float f = (mx - t.X) / (float)t.Width;
            f = Math.Max(0f, Math.Min(1f, f));
            int v = min + (int)Math.Round(f * (max - min));
            if (v != val) { val = v; Invalidate(); cb(v); }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { drag = true; Capture = true; SetFromMouse(e.X); Invalidate(); }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (drag) SetFromMouse(e.X);
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            drag = false; Capture = false; Invalidate();
            base.OnMouseUp(e);
        }
    }

    public class ChoiceRow : Row
    {
        string[] opts; int idx; Action<int> cb;
        public ChoiceRow(string glyph, string caption, string[] opts, int value, Action<int> cb) : base(glyph, caption, 32)
        { this.opts = opts; idx = Math.Max(0, Math.Min(opts.Length - 1, value)); this.cb = cb; Cursor = Cursors.Hand; }
        Rectangle Chip { get { return new Rectangle(Width - 150, 3, 150, Height - 6); } }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Height, Theme.Text, Width - 156);
            Rectangle c = Chip;
            Theme.FillRound(g, Hover ? Theme.ChipHover : Theme.Chip, c, 6);
            Theme.Label(g, "\u2039", Theme.Title, Theme.Dim, new Rectangle(c.X + 4, c.Y, 20, c.Height), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, "\u203A", Theme.Title, Theme.Dim, new Rectangle(c.Right - 24, c.Y, 20, c.Height), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, opts[idx], Theme.Normal, Theme.Text, new Rectangle(c.X + 22, c.Y, c.Width - 44, c.Height), TextFormatFlags.HorizontalCenter);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Rectangle c = Chip;
                bool prev = e.X >= c.X && e.X < c.X + c.Width / 2;
                idx = prev ? (idx - 1 + opts.Length) % opts.Length : (idx + 1) % opts.Length;
                Invalidate();
                cb(idx);
            }
            base.OnMouseClick(e);
        }
    }

    public class StyleRow : Row
    {
        Settings s; Action<int> cb; bool open; int hoverTile = -1;
        const int Head = 34, TileH = 66, Cols = 4, Gap = 6;
        public StyleRow(string glyph, string caption, Settings s, Action<int> cb) : base(glyph, caption, Head)
        { this.s = s; this.cb = cb; Cursor = Cursors.Hand; }

        int RowsCount { get { return (Settings.StyleNames.Length + Cols - 1) / Cols; } }
        int OpenHeight { get { return Head + 6 + RowsCount * (TileH + Gap); } }

        Rectangle TileRect(int i)
        {
            int w = (Width - Gap * (Cols - 1)) / Cols;
            int r = i / Cols, c = i % Cols;
            return new Rectangle(c * (w + Gap), Head + 6 + r * (TileH + Gap), w, TileH);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Head, Theme.Text, Width - 150);
            Rectangle chip = new Rectangle(Width - 140, 3, 140, Head - 6);
            Theme.FillRound(g, (Hover && hoverTile < 0) || open ? Theme.ChipHover : Theme.Chip, chip, 6);
            Theme.Label(g, Settings.StyleNames[s.Style], Theme.Normal, Theme.Text, new Rectangle(chip.X + 12, chip.Y, chip.Width - 40, chip.Height), TextFormatFlags.Left);
            Rectangle arrow = new Rectangle(chip.Right - 28, chip.Y, 20, chip.Height);
            if (Theme.HasIcons) Theme.Glyph(g, open ? "\uE70E" : "\uE70D", Theme.Dim, arrow);
            else Theme.Label(g, open ? "^" : "v", Theme.Normal, Theme.Dim, arrow, TextFormatFlags.HorizontalCenter);

            if (!open) return;
            for (int i = 0; i < Settings.StyleNames.Length; i++)
            {
                Rectangle t = TileRect(i);
                bool sel = i == s.Style;
                Theme.FillRound(g, sel ? Color.FromArgb(42, 42, 50) : (i == hoverTile ? Color.FromArgb(34, 34, 40) : Color.FromArgb(28, 28, 33)), t, 6);
                Rectangle pr = new Rectangle(t.X + 5, t.Y + 5, t.Width - 10, t.Height - 28);
                Theme.FillRound(g, Color.FromArgb(36, 36, 42), pr, 3);

                Settings c = s.Clone();
                c.Style = i; c.OffsetX = 0; c.OffsetY = 0;
                float sc = (pr.Height / 2f - 3) / (float)(Math.Max(0, c.Gap) + Math.Max(1, c.Length) + 2);
                sc = Math.Max(0.6f, Math.Min(2f, sc));
                GraphicsState st = g.Save();
                g.SetClip(pr);
                g.TranslateTransform(pr.X + pr.Width / 2f, pr.Y + pr.Height / 2f);
                g.ScaleTransform(sc, sc);
                Crosshair.Draw(g, c, 0, 0);
                g.Restore(st);

                Theme.Label(g, Settings.StyleNames[i], Theme.Tiny, sel ? Theme.Text : Theme.Dim,
                            new Rectangle(t.X, t.Bottom - 22, t.Width, 20), TextFormatFlags.HorizontalCenter);
                if (sel) Theme.DrawRound(g, Color.FromArgb(200, 200, 210), t, 6);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = -1;
            if (open)
                for (int i = 0; i < Settings.StyleNames.Length; i++)
                    if (TileRect(i).Contains(e.Location)) h = i;
            if (h != hoverTile) { hoverTile = h; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { hoverTile = -1; base.OnMouseLeave(e); }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (e.Y < Head)
                {
                    open = !open;
                    Height = open ? OpenHeight : Head;
                    Invalidate();
                }
                else if (open)
                {
                    for (int i = 0; i < Settings.StyleNames.Length; i++)
                        if (TileRect(i).Contains(e.Location)) { cb(i); Invalidate(); }
                }
            }
            base.OnMouseClick(e);
        }
    }

    public class ProfileRow : Row
    {
        bool active, canDelete; int hoverBtn = -1;
        Func<bool> dirty; Action use, save, del;
        public ProfileRow(string name, bool active, bool canDelete, Func<bool> dirty, Action use, Action save, Action del)
            : base("X", name, 40)
        { this.active = active; this.canDelete = canDelete; this.dirty = dirty; this.use = use; this.save = save; this.del = del; }

        Rectangle DelRect { get { return new Rectangle(Width - 34, 6, 28, Height - 12); } }
        Rectangle SaveRect { get { return new Rectangle(Width - 34 - 8 - 84, 6, 84, Height - 12); } }
        Rectangle UseRect { get { return new Rectangle(SaveRect.X - 8 - 96, 6, 96, Height - 12); } }

        void Btn(Graphics g, Rectangle r, string text, bool hover)
        {
            Theme.FillRound(g, hover ? Color.FromArgb(52, 52, 60) : Color.FromArgb(38, 38, 45), r, 6);
            Theme.Label(g, text, Theme.Normal, Theme.Text, r, TextFormatFlags.HorizontalCenter);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            if (active) Theme.FillRound(g, Color.FromArgb(34, 34, 40), r, 6);
            else if (Hover) Theme.FillRound(g, Color.FromArgb(29, 29, 35), r, 6);
            PaintCaption(g, 10, 0, Height, active ? Theme.Text : Theme.Dim, UseRect.X - 18);

            if (active)
            {
                bool d = dirty();
                Rectangle c = UseRect;
                Theme.FillRound(g, d ? Color.FromArgb(70, 50, 18) : Color.FromArgb(22, 64, 42), c, 6);
                Theme.Label(g, d ? "gewijzigd" : "actief", Theme.Normal, d ? Color.FromArgb(255, 190, 90) : Color.FromArgb(120, 240, 170), c, TextFormatFlags.HorizontalCenter);
            }
            else Btn(g, UseRect, "Gebruiken", hoverBtn == 0);

            Btn(g, SaveRect, "Opslaan", hoverBtn == 1);

            if (canDelete && !active)
            {
                Rectangle dr = DelRect;
                if (hoverBtn == 2) Theme.FillRound(g, Color.FromArgb(60, 30, 34), dr, 5);
                if (Theme.HasIcons) Theme.Glyph(g, "\uE74D", hoverBtn == 2 ? Theme.Danger : Theme.Dim, dr);
                else Theme.Label(g, "x", Theme.Bold, hoverBtn == 2 ? Theme.Danger : Theme.Dim, dr, TextFormatFlags.HorizontalCenter);
            }
        }

        int HitTest(Point p)
        {
            if (!active && UseRect.Contains(p)) return 0;
            if (SaveRect.Contains(p)) return 1;
            if (canDelete && !active && DelRect.Contains(p)) return 2;
            return -1;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = HitTest(e.Location);
            if (h != hoverBtn) { hoverBtn = h; Cursor = h >= 0 ? Cursors.Hand : Cursors.Default; Invalidate(); }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e) { hoverBtn = -1; base.OnMouseLeave(e); }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int h = HitTest(e.Location);
                if (h == 0) use();
                else if (h == 1) save();
                else if (h == 2) del();
            }
            base.OnMouseClick(e);
        }
    }

    public class InputRow : Ctl
    {
        TextBox box;
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, string l);
        public InputRow(string placeholder, Action enter)
        {
            box = new TextBox();
            box.BorderStyle = BorderStyle.None;
            box.BackColor = Theme.Chip;
            box.ForeColor = Theme.Text;
            box.Font = Theme.Normal;
            box.MaxLength = 24;
            box.KeyDown += (o, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; enter(); } };
            box.HandleCreated += delegate { SendMessage(box.Handle, 0x1501, (IntPtr)1, placeholder); };
            box.GotFocus += delegate { Invalidate(); };
            box.LostFocus += delegate { Invalidate(); };
            Controls.Add(box);
            Cursor = Cursors.IBeam;
            Height = 34;
        }
        public string Value { get { return box.Text; } set { box.Text = value; } }
        public void FocusBox() { box.Focus(); box.SelectAll(); }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (box == null) return;
            box.Left = 12; box.Width = Math.Max(10, Width - 24);
            box.Top = (Height - box.Height) / 2;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            Theme.FillRound(e.Graphics, Theme.Chip, r, 6);
            Theme.DrawRound(e.Graphics, box.Focused ? Color.FromArgb(120, 120, 132) : Theme.Border, r, 6);
        }
        protected override void OnMouseClick(MouseEventArgs e) { box.Focus(); base.OnMouseClick(e); }
    }

    public class CodeRow : Ctl
    {
        Func<string> get;
        static readonly Font Mono = new Font("Consolas", 10f);
        public CodeRow(Func<string> get) { this.get = get; Height = 34; }
        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            Theme.FillRound(e.Graphics, Color.FromArgb(14, 14, 17), r, 6);
            Theme.DrawRound(e.Graphics, Theme.Border, r, 6);
            Theme.Label(e.Graphics, get(), Mono, Color.FromArgb(120, 240, 170), new Rectangle(12, 0, Width - 24, Height), TextFormatFlags.Left);
        }
    }

    public class AppRow : Row
    {
        Func<string> get; Action<string> set;
        public AppRow(string glyph, string caption, Func<string> get, Action<string> set) : base(glyph, caption, 32)
        { this.get = get; this.set = set; Cursor = Cursors.Hand; }
        Rectangle Chip { get { return new Rectangle(Width - 150, 3, 150, Height - 6); } }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Height, Theme.Text, Width - 156);
            Rectangle c = Chip;
            Theme.FillRound(g, Hover ? Theme.ChipHover : Theme.Chip, c, 6);
            string v = get();
            Theme.Label(g, string.IsNullOrEmpty(v) ? "Alle apps" : v, Theme.Normal, Theme.Text, new Rectangle(c.X + 12, c.Y, c.Width - 40, c.Height), TextFormatFlags.Left);
            Rectangle arrow = new Rectangle(c.Right - 28, c.Y, 20, c.Height);
            if (Theme.HasIcons) Theme.Glyph(g, "\uE70D", Theme.Dim, arrow);
            else Theme.Label(g, "v", Theme.Normal, Theme.Dim, arrow, TextFormatFlags.HorizontalCenter);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ContextMenuStrip cms = new ContextMenuStrip();
                cms.Font = Theme.Normal;
                cms.Items.Add("Alle apps (altijd tonen)", null, delegate { set(""); Invalidate(); });
                cms.Items.Add(new ToolStripSeparator());
                List<string> seen = new List<string>();
                List<string[]> apps = new List<string[]>();
                int own = System.Diagnostics.Process.GetCurrentProcess().Id;
                foreach (System.Diagnostics.Process pr in System.Diagnostics.Process.GetProcesses())
                using (pr)
                {
                    try
                    {
                        if (pr.Id == own || pr.MainWindowHandle == IntPtr.Zero) continue;
                        string title = pr.MainWindowTitle;
                        if (string.IsNullOrEmpty(title)) continue;
                        string name = pr.ProcessName;
                        if (seen.Contains(name.ToLower())) continue;
                        seen.Add(name.ToLower());
                        if (title.Length > 40) title = title.Substring(0, 40) + "...";
                        apps.Add(new string[] { name, title });
                    }
                    catch { }
                }
                apps.Sort((x, y) => string.Compare(x[0], y[0], StringComparison.OrdinalIgnoreCase));
                foreach (string[] a in apps)
                {
                    string name = a[0];
                    ToolStripItem it = cms.Items.Add(name + "   (" + a[1] + ")", null, delegate { set(name); Invalidate(); });
                    if (string.Equals(name, get(), StringComparison.OrdinalIgnoreCase)) ((ToolStripMenuItem)it).Checked = true;
                }
                if (apps.Count == 0) { ToolStripItem none = cms.Items.Add("Geen apps gevonden"); none.Enabled = false; }
                if (string.IsNullOrEmpty(get())) ((ToolStripMenuItem)cms.Items[0]).Checked = true;
                Rectangle c = Chip;
                cms.Closed += delegate { BeginInvoke(new Action(cms.Dispose)); };
                cms.Show(this, new Point(c.X, c.Bottom + 2));
            }
            base.OnMouseClick(e);
        }
    }

    public class PixelEditor : Ctl
    {
        Settings s; Action changed; bool painting; char brush;
        const int N = Settings.Grid;
        public bool EraserTool = false;

        int CellIndex(int mx, int my)
        {
            int c = Cell;
            int x = (mx - Ox) / c, y = my / c;
            if (mx < Ox || x < 0 || y < 0 || x >= N || y >= N) return -1;
            return y * N + x;
        }
        public PixelEditor(Settings s, Action changed) { this.s = s; this.changed = changed; Height = 250; Cursor = Cursors.Cross; }
        int Cell { get { return Math.Max(4, Math.Min(Width, 300) / N); } }
        int Ox { get { return (Width - Cell * N) / 2; } }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int h = Cell * N + 1;
            if (Height != h) Height = h;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int c = Cell, ox = Ox;
            using (SolidBrush bg = new SolidBrush(Color.FromArgb(14, 14, 17))) g.FillRectangle(bg, ox, 0, c * N, c * N);
            int mid = N / 2;
            using (SolidBrush cb = new SolidBrush(Color.FromArgb(30, 30, 36))) g.FillRectangle(cb, ox + mid * c, mid * c, c, c);
            using (SolidBrush fb = new SolidBrush(Crosshair.ColorFor(s)))
                for (int i = 0; i < N * N && i < s.Pixels.Length; i++)
                    if (s.Pixels[i] == '1') g.FillRectangle(fb, ox + (i % N) * c + 1, (i / N) * c + 1, c - 1, c - 1);
            using (Pen p = new Pen(Color.FromArgb(34, 34, 40)))
                for (int i = 0; i <= N; i++)
                {
                    g.DrawLine(p, ox + i * c, 0, ox + i * c, N * c);
                    g.DrawLine(p, ox, i * c, ox + N * c, i * c);
                }
            using (Pen mp = new Pen(Color.FromArgb(90, 90, 100)))
                g.DrawRectangle(mp, ox + mid * c, mid * c, c, c);
        }
        void PaintAt(int mx, int my)
        {
            int c = Cell;
            int x = (mx - Ox) / c, y = my / c;
            if (mx < Ox || x < 0 || y < 0 || x >= N || y >= N) return;
            char[] a = s.Pixels.ToCharArray();
            int i = y * N + x;
            if (a[i] == brush) return;
            a[i] = brush;
            s.Pixels = new string(a);
            s.Style = Settings.StylePixels;
            Invalidate();
            changed();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
            {
                painting = true; Capture = true;
                brush = e.Button == MouseButtons.Right ? '0' : '1';
                PaintAt(e.X, e.Y);
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e) { if (painting) PaintAt(e.X, e.Y); base.OnMouseMove(e); }
        protected override void OnMouseUp(MouseEventArgs e) { painting = false; Capture = false; base.OnMouseUp(e); }
    }

    public class ThemeTiles : Ctl
    {
        Func<int> get; Action<int> set; int hover = -1;
        const int TH = 104, Gap = 10;
        public ThemeTiles(Func<int> get, Action<int> set)
        {
            this.get = get; this.set = set; Cursor = Cursors.Hand;
            Height = 2 * (TH + Gap);
        }
        int Cols { get { return Width >= 380 ? 3 : 2; } }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int rows = (Theme.StyleNames.Length + Cols - 1) / Cols;
            int h = rows * (TH + Gap) - Gap;
            if (Width > 0 && Height != h) Height = h;
        }
        Rectangle Tile(int i)
        {
            int w = (Width - Gap * (Cols - 1)) / Cols;
            return new Rectangle((i % Cols) * (w + Gap), (i / Cols) * (TH + Gap), w, TH);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int i = 0; i < Theme.StyleNames.Length; i++)
            {
                Rectangle r = Tile(i);
                bool sel = get() == i;
                Color[] pal = Theme.PaletteFor(i);
                Theme.FillRound(g, hover == i ? Theme.Hover : Theme.Chip, r, 8);
                if (sel) { Theme.DrawRound(g, Theme.Accent, r, 8); Theme.DrawRound(g, Theme.Accent, Rectangle.Inflate(r, -1, -1), 7); }
                Rectangle m = new Rectangle(r.X + 10, r.Y + 10, r.Width - 20, 60);
                Theme.FillRound(g, pal[0], m, 5);
                Theme.DrawRound(g, pal[3], m, 5);
                if (i == 4)
                {
                    Theme.FillRound(g, pal[1], new Rectangle(m.X + 1, m.Y + 1, m.Width - 2, 14), 4);
                    using (SolidBrush ab = new SolidBrush(Theme.Accent)) g.FillEllipse(ab, m.X + 6, m.Y + 4, 7, 7);
                    using (SolidBrush lb = new SolidBrush(pal[3]))
                    {
                        g.FillRectangle(lb, m.X + 20, m.Y + 6, 16, 3);
                        g.FillRectangle(lb, m.X + 40, m.Y + 6, 16, 3);
                    }
                    Rectangle gc = new Rectangle(m.X + 6, m.Y + 22, m.Width - 12, m.Height - 28);
                    Theme.FillRound(g, pal[2], gc, 6);
                    using (Pen hl = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(hl, gc.X + 4, gc.Y + 1, gc.Right - 4, gc.Y + 1);
                    Theme.FillRound(g, Theme.Accent, new RectangleF(gc.Right - 20, gc.Bottom - 12, 14, 7), 3.5f);
                    Theme.Label(g, Theme.StyleNames[i], Theme.Bold, sel ? Theme.Text : Theme.Dim,
                                new Rectangle(r.X, r.Bottom - 30, r.Width, 24), TextFormatFlags.HorizontalCenter);
                    continue;
                }
                using (SolidBrush sb = new SolidBrush(pal[1])) g.FillRectangle(sb, m.X + 1, m.Y + 3, 32, m.Height - 6);
                using (SolidBrush ab = new SolidBrush(Theme.Accent)) g.FillRectangle(ab, m.X + 4, m.Y + 20, 2, 8);
                using (SolidBrush lb = new SolidBrush(pal[3]))
                {
                    g.FillRectangle(lb, m.X + 9, m.Y + 22, 18, 3);
                    g.FillRectangle(lb, m.X + 9, m.Y + 32, 18, 3);
                    g.FillRectangle(lb, m.X + 9, m.Y + 42, 18, 3);
                }
                Rectangle c = new Rectangle(m.X + 38, m.Y + 20, m.Width - 44, m.Height - 26);
                Theme.FillRound(g, pal[2], c, i == 1 ? 2 : 4);
                if (i == 1)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (LinearGradientBrush lg = new LinearGradientBrush(new Rectangle(m.Right - 60, m.Y + 5, 56, 12), Color.FromArgb(200, 200, 206), Color.FromArgb(244, 244, 247), 0f))
                        g.FillPolygon(lg, new PointF[] { new PointF(m.Right - 56, m.Y + 5), new PointF(m.Right - 4, m.Y + 5), new PointF(m.Right - 4, m.Y + 15), new PointF(m.Right - 60, m.Y + 15) });
                    using (SolidBrush wb = new SolidBrush(Theme.Text)) g.FillRectangle(wb, c.X + 6, c.Y + 13, 12, 2);
                }
                using (SolidBrush tb = new SolidBrush(Color.FromArgb(110, 110, 120))) g.FillRectangle(tb, c.X + 6, c.Y + 6, 30, 4);
                Theme.FillRound(g, Theme.Accent, new RectangleF(c.Right - 20, c.Bottom - 12, 14, 7), 3.5f);
                Theme.Label(g, Theme.StyleNames[i], i == 1 ? Theme.HeavySmall : Theme.Bold, sel ? Theme.Text : Theme.Dim,
                            new Rectangle(r.X, r.Bottom - 30, r.Width, 24), TextFormatFlags.HorizontalCenter);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = -1;
            for (int i = 0; i < Theme.StyleNames.Length; i++) if (Tile(i).Contains(e.Location)) h = i;
            if (h != hover) { hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { hover = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            for (int i = 0; i < Theme.StyleNames.Length; i++) if (Tile(i).Contains(e.Location) && get() != i) { set(i); break; }
            base.OnMouseClick(e);
        }
    }

    public class StyleGrid : Ctl
    {
        Settings s; Action<int> cb; int hoverTile = -1;
        const int TileH = 64, Gap = 8, HeadH = 24;
        static readonly string[] Groups = new string[] { "Basis", "Vormen", "Speciaal" };
        static readonly int[][] Items = new int[][] {
            new int[] { 0, 2, 1, 5, 20, 22 },
            new int[] { 6, 7, 8, 9, 16, 17, 10 },
            new int[] { 11, 12, 21, 13 } };
        string[] groups; int[][] items; int headH; int minW;
        List<int> tileStyle = new List<int>();
        List<Rectangle> tileRect = new List<Rectangle>();
        List<Point> heads = new List<Point>();

        public StyleGrid(Settings s, Action<int> cb) : this(s, cb, null) { }
        public StyleGrid(Settings s, Action<int> cb, int[] flat)
        {
            this.s = s; this.cb = cb; Height = 300; Cursor = Cursors.Hand;
            if (flat == null) { groups = Groups; items = Items; headH = HeadH; minW = 76; }
            else { groups = new string[] { "" }; items = new int[][] { flat }; headH = 0; minW = 56; }
        }

        void Layout2()
        {
            tileStyle.Clear(); tileRect.Clear(); heads.Clear();
            if (Width <= 0) return;
            int cols = Math.Max(3, (Width + Gap) / (minW + Gap));
            int w = (Width - Gap * (cols - 1)) / cols;
            int y = 0;
            for (int gi = 0; gi < groups.Length; gi++)
            {
                heads.Add(new Point(0, y));
                y += headH;
                int[] its = items[gi];
                for (int k = 0; k < its.Length; k++)
                {
                    int col = k % cols, row = k / cols;
                    tileStyle.Add(its[k]);
                    tileRect.Add(new Rectangle(col * (w + Gap), y + row * (TileH + Gap), w, TileH));
                }
                int rows = (its.Length + cols - 1) / cols;
                y += rows * (TileH + Gap) + 4;
            }
            if (Height != y) Height = y;
        }

        protected override void OnResize(EventArgs e) { base.OnResize(e); Layout2(); Invalidate(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (tileRect.Count == 0) Layout2();
            for (int gi = 0; gi < groups.Length && gi < heads.Count && headH > 0; gi++)
            {
                string h = Theme.Lounge ? Theme.Caps(groups[gi]) : groups[gi].ToUpper();
                Theme.Label(g, h, Theme.Small, Theme.Faint, new Rectangle(2, heads[gi].Y, Width, HeadH - 4), TextFormatFlags.Left);
            }
            for (int i = 0; i < tileRect.Count; i++)
            {
                Rectangle t = tileRect[i];
                int style = tileStyle[i];
                bool sel = style == s.Style;
                Theme.FillRound(g, sel ? Theme.Selected : (i == hoverTile ? Theme.Hover : Theme.Chip), t, 7);
                Rectangle pr = new Rectangle(t.X + 5, t.Y + 5, t.Width - 10, t.Height - 26);
                Theme.FillRound(g, Theme.Window, pr, 4);
                Settings c = s.Clone();
                c.Style = style; c.OffsetX = 0; c.OffsetY = 0;
                float sc = (pr.Height / 2f - 2) / (float)(Math.Max(0, c.Gap) + Math.Max(1, c.Length) + 4);
                sc = Math.Max(0.5f, Math.Min(1.6f, sc));
                GraphicsState st = g.Save();
                g.SetClip(pr);
                g.TranslateTransform(pr.X + pr.Width / 2f, pr.Y + pr.Height / 2f);
                g.ScaleTransform(sc, sc);
                Crosshair.Draw(g, c, 0, 0);
                g.Restore(st);
                Theme.Label(g, Settings.StyleNames[style], Theme.Tiny, sel ? Theme.Text : Theme.Dim,
                            new Rectangle(t.X + 2, t.Bottom - 21, t.Width - 4, 18), TextFormatFlags.HorizontalCenter);
                if (sel) Theme.DrawRound(g, Theme.Accent, t, 7);
            }
        }

        int Hit(Point p)
        {
            for (int i = 0; i < tileRect.Count; i++) if (tileRect[i].Contains(p)) return i;
            return -1;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = Hit(e.Location);
            if (h != hoverTile) { hoverTile = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { hoverTile = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int h = Hit(e.Location);
            if (e.Button == MouseButtons.Left && h >= 0) { cb(tileStyle[h]); Invalidate(); }
            base.OnMouseClick(e);
        }
    }

    public class TwoButtons : Ctl
    {
        string a, b; Action ca, cb; int hover = -1;
        public TwoButtons(string a, Action ca, string b, Action cb) { this.a = a; this.b = b; this.ca = ca; this.cb = cb; Height = 34; Cursor = Cursors.Hand; }
        Rectangle R(int i) { int w = (Width - 8) / 2; return new Rectangle(i * (w + 8), 0, w, Height - 1); }
        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                Rectangle r = R(i);
                bool primary = i == 0;
                Color bg = primary ? (hover == i ? ControlPaint.Light(Theme.Accent, 0.4f) : Theme.Accent) : (hover == i ? Theme.ChipHover : Theme.Chip);
                Theme.FillRound(e.Graphics, bg, r, 6);
                Theme.Label(e.Graphics, i == 0 ? a : b, Theme.Bold, primary ? Theme.AccentText : Theme.Text, r, TextFormatFlags.HorizontalCenter);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = R(0).Contains(e.Location) ? 0 : (R(1).Contains(e.Location) ? 1 : -1);
            if (h != hover) { hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { hover = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (R(0).Contains(e.Location)) ca();
                else if (R(1).Contains(e.Location)) cb();
            }
            base.OnMouseClick(e);
        }
    }

    public class ServerRow : Ctl
    {
        FServer f; Action join, remove, profile; int hover = -1;
        public ServerRow(FServer f, Action join, Action remove, Action profile)
        { this.f = f; this.join = join; this.remove = remove; this.profile = profile; Height = 80; }
        Rectangle JoinR { get { return new Rectangle(Width - 86, 22, 74, 32); } }
        Rectangle DelR { get { return new Rectangle(Width - 116, 26, 24, 24); } }
        Rectangle ProfR { get { return new Rectangle(34, 52, 220, 18); } }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Theme.FillRound(g, hover >= 0 ? Theme.ChipHover : Theme.Chip, new RectangleF(0, 0, Width - 1, Height - 1), 10);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color dot = !f.Loaded ? Theme.Dim : (f.Online ? Color.FromArgb(52, 211, 120) : Color.FromArgb(239, 90, 90));
            using (SolidBrush b = new SolidBrush(dot)) g.FillEllipse(b, 14, 15, 10, 10);
            Theme.Label(g, f.Name, Theme.Bold, Theme.Text, new Rectangle(34, 8, Width - 160, 24), TextFormatFlags.Left);
            string info = !f.Loaded ? "laden..." : (f.Online ? "Online  -  " + f.Clients + " / " + f.Max + " spelers" : "Offline");
            Theme.Label(g, info, Theme.Tiny, Theme.Dim, new Rectangle(34, 30, Width - 160, 18), TextFormatFlags.Left);
            float fr = f.Max > 0 ? Math.Min(1f, f.Clients / (float)f.Max) : 0f;
            RectangleF bar = new RectangleF(34, 50, Math.Max(40, Width - 280), 4);
            Theme.FillRound(g, Theme.Track, bar, 2);
            if (fr > 0) Theme.FillRound(g, Theme.Accent, new RectangleF(bar.X, bar.Y, bar.Width * fr, bar.Height), 2);
            string pt = string.IsNullOrEmpty(f.Profile) ? "geen profiel gekoppeld" : "profiel: " + f.Profile;
            Theme.Label(g, pt + "  \u25BE", Theme.Tiny, hover == 2 ? Theme.Text : Theme.Dim, new Rectangle((int)bar.Right + 10, 42, 150, 20), TextFormatFlags.Left);
            bool on = f.Loaded && f.Online;
            Rectangle j = JoinR;
            Theme.FillRound(g, on ? (hover == 0 ? ControlPaint.Light(Theme.Accent, 0.4f) : Theme.Accent) : Theme.Track, j, 8);
            Theme.Label(g, "Join", Theme.Bold, on ? Theme.AccentText : Theme.Dim, j, TextFormatFlags.HorizontalCenter);
            if (hover >= 0)
            {
                if (Theme.HasIcons) Theme.Glyph(g, "\uE74D", hover == 1 ? Theme.Danger : Theme.Dim, DelR);
                else Theme.Label(g, "x", Theme.Bold, hover == 1 ? Theme.Danger : Theme.Dim, DelR, TextFormatFlags.HorizontalCenter);
            }
        }
        Rectangle ProfHit { get { return new Rectangle(Math.Max(74, Width - 280) + 44, 40, 150, 24); } }
        int Hit(Point p)
        {
            if (JoinR.Contains(p)) return 0;
            if (DelR.Contains(p)) return 1;
            if (ProfHit.Contains(p)) return 2;
            return 3;
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            int h = Hit(e.Location);
            Cursor = h <= 2 ? Cursors.Hand : Cursors.Default;
            if (h != hover) { hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { hover = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int h = Hit(e.Location);
                if (h == 0) join();
                else if (h == 1) remove();
                else if (h == 2) profile();
            }
            base.OnMouseClick(e);
        }
    }

    public class NewsButton : Ctl
    {
        Func<bool> unseen; Action click;
        public NewsButton(Func<bool> unseen, Action click) { this.unseen = unseen; this.click = click; Height = 32; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            Theme.FillRound(g, Hover ? Theme.Hover : Theme.Card, r, 9);
            Theme.Glyph(g, "\uE789", Theme.Accent, new Rectangle(12, 0, 20, Height));
            Theme.Label(g, "Nieuws", Theme.Bold, Theme.Text, new Rectangle(40, 0, 100, Height), TextFormatFlags.Left);
            if (unseen())
            {
                RectangleF pill = new RectangleF(Width - 58, 8, 48, 16);
                Theme.FillRound(g, Theme.Accent, pill, 8);
                Theme.Label(g, "NIEUW", Theme.Small, Theme.AccentText, Rectangle.Round(pill), TextFormatFlags.HorizontalCenter);
            }
        }
        protected override void OnMouseClick(MouseEventArgs e) { if (e.Button == MouseButtons.Left) click(); base.OnMouseClick(e); }
    }

    public class LinkRow : Ctl
    {
        string text; Action click;
        public LinkRow(string text, Action click) { this.text = text; this.click = click; Height = 26; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Theme.Label(e.Graphics, text, Theme.Bold, Hover ? ControlPaint.Light(Theme.Accent, 0.4f) : Theme.Accent, new Rectangle(0, 0, Width, Height), TextFormatFlags.Left);
        }
        protected override void OnMouseClick(MouseEventArgs e) { if (e.Button == MouseButtons.Left) click(); base.OnMouseClick(e); }
    }

    public class SectionHeader : Ctl
    {
        public string Title;
        public SectionHeader(string title) { Title = title; Height = 34; BackColor = Theme.Window; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillRectangle(b, 0, 9, 3, 18);
            Theme.Label(g, Theme.Up(Title), Theme.Lounge ? Theme.Heavy : Theme.Title, Theme.Text, new Rectangle(12, 2, Width - 12, 32), TextFormatFlags.Left);
        }
    }

    public class JumpBar : Ctl
    {
        string[] names; Action<int> jump; int hover = -1;
        List<Rectangle> rects = new List<Rectangle>();
        public JumpBar(string[] names, Action<int> jump) { this.names = names; this.jump = jump; Height = 32; Cursor = Cursors.Hand; }
        void Calc()
        {
            rects.Clear();
            int x = 0;
            foreach (string n in names)
            {
                int w = TextRenderer.MeasureText(n, Theme.Normal).Width + 22;
                rects.Add(new Rectangle(x, 2, w, Height - 4));
                x += w + 6;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Calc();
            for (int i = 0; i < names.Length; i++)
            {
                Rectangle r = rects[i];
                Theme.FillRound(e.Graphics, hover == i ? Theme.ChipHover : Theme.Chip, r, r.Height / 2f);
                Theme.Label(e.Graphics, names[i], Theme.Normal, hover == i ? Theme.Text : Theme.Dim, r, TextFormatFlags.HorizontalCenter);
            }
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            Calc();
            int h = -1;
            for (int i = 0; i < rects.Count; i++) if (rects[i].Contains(e.Location)) h = i;
            if (h != hover) { hover = h; Invalidate(); }
            base.OnMouseMove(e);
        }
        protected override void OnMouseLeave(EventArgs e) { hover = -1; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Calc();
            for (int i = 0; i < rects.Count; i++) if (rects[i].Contains(e.Location)) { jump(i); break; }
            base.OnMouseClick(e);
        }
    }

    public class KeyRow : Row
    {
        int vk; bool capturing; Action<int> cb;
        public KeyRow(string glyph, string caption, int value, Action<int> cb) : base(glyph, caption, 34)
        { vk = value; this.cb = cb; Cursor = Cursors.Hand; }
        Rectangle Chip { get { return new Rectangle(Width - 112, 3, 112, Height - 6); } }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Height, Theme.Text, Width - 120);
            Rectangle c = Chip;
            Theme.FillRound(g, capturing ? Color.FromArgb(48, 48, 56) : (Hover ? Theme.ChipHover : Theme.Chip), c, 6);
            Theme.Glyph(g, "\uE765", Theme.Dim, new Rectangle(c.X + 8, c.Y, 18, c.Height));
            Theme.Label(g, capturing ? "Druk toets..." : Theme.KeyName(vk), Theme.Bold, Theme.Text,
                        new Rectangle(c.X + 30, c.Y, c.Width - 36, c.Height), TextFormatFlags.Left);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !capturing)
            {
                capturing = true;
                Invalidate();
                Program.BeginCapture(delegate(int k)
                {
                    capturing = false;
                    if (k != 0x1B) { vk = k; cb(k); }
                    if (!IsDisposed) Invalidate();
                });
            }
            base.OnMouseClick(e);
        }
    }

    public class ColorRow : Row
    {
        Func<Color> get; Action click;
        public ColorRow(string glyph, string caption, Func<Color> get, Action click) : base(glyph, caption, 30)
        { this.get = get; this.click = click; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            PaintCaption(g, 0, 0, Height, Theme.Text, Width - 40);
            Rectangle r = new Rectangle(Width - 20, (Height - 18) / 2, 18, 18);
            Color c = get();
            Theme.FillRound(g, Color.FromArgb(255, c.R, c.G, c.B), r, 3);
            Theme.DrawRound(g, Hover ? Color.FromArgb(200, 200, 205) : Color.FromArgb(95, 95, 105), r, 3);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) click();
            base.OnMouseClick(e);
        }
    }

    public class SwatchRow : Ctl
    {
        Color[] colors; Func<Color> get; Action<Color> set;
        public SwatchRow(Color[] colors, Func<Color> get, Action<Color> set)
        { this.colors = colors; this.get = get; this.set = set; Height = 36; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color cur = get();
            for (int i = 0; i < colors.Length; i++)
            {
                float x = i * 34 + 5, y = (Height - 22) / 2f;
                using (SolidBrush b = new SolidBrush(colors[i])) g.FillEllipse(b, x, y, 22, 22);
                if (colors[i].R == cur.R && colors[i].G == cur.G && colors[i].B == cur.B)
                    using (Pen p = new Pen(Color.FromArgb(235, 235, 240), 2f)) g.DrawEllipse(p, x - 3, y - 3, 28, 28);
            }
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            int i = e.X / 34;
            if (e.Button == MouseButtons.Left && i >= 0 && i < colors.Length) { set(colors[i]); Invalidate(); }
            base.OnMouseClick(e);
        }
    }

    public class ButtonRow : Row
    {
        Action click; bool danger;
        public ButtonRow(string glyph, string caption, bool danger, Action click) : base(glyph, caption, 34)
        { this.danger = danger; this.click = click; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (Hover) Theme.FillRound(g, Theme.Chip, new RectangleF(0, 0, Width - 1, Height - 1), 6);
            PaintCaption(g, 8, 0, Height, danger ? Theme.Danger : Theme.Text, Width - 16);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) click();
            base.OnMouseClick(e);
        }
    }

    public class InfoRow : Ctl
    {
        string text;
        public InfoRow(string text, int height) { this.text = text; Height = height; }
        protected override void OnPaint(PaintEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics, text, Theme.Tiny, new Rectangle(0, 0, Width, Height), Theme.Dim,
                TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.NoPadding);
        }
    }

    public class PreviewBox : Ctl
    {
        Settings s;
        int zoom = 3;
        public PreviewBox(Settings s) { this.s = s; Height = 180; }
        public PreviewBox(Settings s, int height, int zoom) { this.s = s; Height = height; this.zoom = zoom; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            using (GraphicsPath p = Theme.Round(r, 8))
            using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(0, 0, Width, Height),
                       Color.FromArgb(52, 52, 60), Color.FromArgb(14, 14, 17), 90f))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillPath(b, p);
            }
            Theme.Label(g, zoom + "x vergroot", Theme.Tiny, Color.FromArgb(150, 150, 160), new Rectangle(10, 6, 120, 18), TextFormatFlags.Left);
            if (!s.Enabled)
                Theme.Label(g, "crosshair staat uit", Theme.Tiny, Color.FromArgb(150, 150, 160), new Rectangle(0, Height - 26, Width, 18), TextFormatFlags.HorizontalCenter);
            GraphicsState st = g.Save();
            g.TranslateTransform(Width / 2, Height / 2);
            g.ScaleTransform(zoom, zoom);
            Crosshair.Draw(g, s, 0, 0);
            g.Restore(st);
        }
    }

    public class Card : Ctl
    {
        string glyph, title;
        public Card(string glyph, string title, int width)
        {
            this.glyph = glyph; this.title = title;
            Width = width; Height = 60;
            BackColor = Theme.Window;
        }
        public T Add<T>(T c) where T : Control
        {
            c.Left = 16; c.Width = Width - 32;
            c.BackColor = Theme.Card;
            Controls.Add(c);
            c.SizeChanged += delegate { Relayout(); };
            Relayout();
            return c;
        }
        public void RemovePreviews()
        {
            for (int i = Controls.Count - 1; i >= 0; i--) if (Controls[i] is PreviewBox) Controls[i].Dispose();
            Relayout();
        }
        public bool IsEmpty { get { return Controls.Count == 0; } }

        void Relayout()
        {
            int yy = 48;
            foreach (Control c in Controls) { c.Top = yy; yy += c.Height + 4; }
            Height = yy + 12;
            Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            float rad = Theme.Lounge ? 5 : (Theme.Glass ? 12 : 9);
            Theme.FillRound(g, Theme.Card, r, rad);
            Theme.DrawRound(g, Theme.Border, r, rad);
            if (Theme.Glass)
                using (Pen hl = new Pen(Color.FromArgb(40, 255, 255, 255))) g.DrawLine(hl, rad, 1, Width - rad - 1, 1);
            Theme.Glyph(g, glyph, Theme.Lounge ? Theme.Dim : Theme.Accent, new Rectangle(16, 12, 18, 24));
            Theme.Label(g, Theme.Up(title), Theme.Header, Theme.Text, new Rectangle(42, 12, Width - 60, 24), TextFormatFlags.Left);
            if (Theme.Lounge) Theme.Bar(g, Theme.Text, 43, 35, 26);
        }
    }

    public class NavItem : Ctl
    {
        string glyph, caption; Action click;
        public bool Selected;
        public int PageIndex;
        public int SideTop;
        public string Caption2 { get { return caption; } }
        public NavItem(string glyph, string caption, Action click)
        {
            this.glyph = glyph; this.caption = caption; this.click = click;
            BackColor = Theme.Sidebar; Height = 36; Width = 170; Cursor = Cursors.Hand;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            if (Theme.Glass)
            {
                if (Selected)
                {
                    Theme.FillRound(g, Theme.Card, r, 10);
                    using (SolidBrush ab = new SolidBrush(Theme.Accent)) g.FillRectangle(ab, 0, 9, 3, Height - 18);
                }
                else if (Hover) Theme.FillRound(g, Theme.Hover, r, 10);
                Theme.Glyph(g, glyph, Selected ? Theme.Accent : Theme.Dim, new Rectangle(14, 0, 20, Height));
                Theme.Label(g, caption, Selected ? Theme.Bold : Theme.Normal, Selected ? Theme.Text : Theme.Dim, new Rectangle(44, 0, Width - 48, Height), TextFormatFlags.Left);
                return;
            }
            if (Selected)
            {
                Theme.FillRound(g, Color.FromArgb(30, 30, 35), r, 4);
                using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillRectangle(b, 0, 8, 3, Height - 16);
            }
            else if (Hover) Theme.FillRound(g, Theme.Hover, r, 4);
            Color c = Selected ? Theme.Text : Theme.Dim;
            Theme.Glyph(g, glyph, c, new Rectangle(14, 0, 20, Height));
            Theme.Label(g, caption, Selected ? Theme.Bold : Theme.Normal, c, new Rectangle(44, 0, Width - 50, Height), TextFormatFlags.Left);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) click();
            base.OnMouseClick(e);
        }
    }

    public class TabButton : Ctl
    {
        string glyph, caption; bool selected; Action click;
        public TabButton(string glyph, string caption, bool selected, Action click)
        {
            this.glyph = glyph; this.caption = caption; this.selected = selected; this.click = click;
            BackColor = Theme.Window; Height = 32; Cursor = Cursors.Hand;
            Width = TextRenderer.MeasureText(Theme.Up(caption), Theme.Lounge ? Theme.HeavySmall : Theme.Bold).Width + (Theme.Lounge ? 58 : 50);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            if (!Theme.Lounge)
            {
                if (selected) Theme.FillRound(g, Theme.Selected, r, 7);
                else if (Hover) Theme.FillRound(g, Theme.Hover, r, 7);
                Color cc = selected ? Theme.Text : Theme.Dim;
                Theme.Glyph(g, glyph, selected ? Theme.Accent : cc, new Rectangle(12, 0, 18, Height));
                Theme.Label(g, caption, selected ? Theme.Bold : Theme.Normal, cc, new Rectangle(36, 0, Width - 40, Height), TextFormatFlags.Left);
                return;
            }
            if (selected || Hover)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                PointF[] pts = new PointF[] { new PointF(8, 0), new PointF(Width, 0), new PointF(Width - 8, Height), new PointF(0, Height) };
                if (selected)
                    using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(0, 0, Width, Height), Color.FromArgb(240, 240, 244), Color.FromArgb(198, 198, 205), 0f))
                        g.FillPolygon(b, pts);
                else
                    using (SolidBrush b = new SolidBrush(Theme.Hover)) g.FillPolygon(b, pts);
            }
            Color c = selected ? Color.FromArgb(18, 18, 22) : Theme.Dim;
            Theme.Glyph(g, glyph, c, new Rectangle(14, 0, 18, Height));
            Theme.Label(g, caption.ToUpper(), Theme.HeavySmall, c, new Rectangle(38, 0, Width - 40, Height), TextFormatFlags.Left);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) click();
            base.OnMouseClick(e);
        }
    }

    public class CloseButton : Ctl
    {
        Action click;
        public CloseButton(Action click) { this.click = click; BackColor = Theme.Window; Size = new Size(32, 32); Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (Hover) Theme.FillRound(g, Theme.Hover, new RectangleF(0, 0, Width - 1, Height - 1), 6);
            if (Theme.HasIcons) Theme.Glyph(g, "\uE711", Hover ? Theme.Text : Theme.Dim, new Rectangle(0, 0, Width, Height));
            else Theme.Label(g, "X", Theme.Bold, Theme.Dim, new Rectangle(0, 0, Width, Height), TextFormatFlags.HorizontalCenter);
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) click();
            base.OnMouseClick(e);
        }
    }

    public class PrimaryButton : Ctl
    {
        string caption; Action click;
        public PrimaryButton(string caption, Action click) { this.caption = caption; this.click = click; Height = 36; Cursor = Cursors.Hand; }
        protected override void OnPaint(PaintEventArgs e)
        {
            RectangleF r = new RectangleF(0, 0, Width - 1, Height - 1);
            Color bg = Hover ? ControlPaint.Light(Theme.Accent, 0.4f) : Theme.Accent;
            Theme.FillRound(e.Graphics, bg, r, 7);
            Theme.Label(e.Graphics, caption, Theme.Bold, Theme.AccentText, new Rectangle(0, 0, Width, Height), TextFormatFlags.HorizontalCenter);
        }
        protected override void OnMouseClick(MouseEventArgs e) { if (e.Button == MouseButtons.Left) click(); base.OnMouseClick(e); }
    }

    public class NameForm : Form
    {
        InputRow input; Label error; Bitmap logo = AppIcon.MakeBitmap(112);
        public string Result = "";

        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        [DllImport("user32.dll")] static extern bool ReleaseCapture();
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);

        public NameForm(string current)
        {
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(380, 250);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = true;
            Text = "Custom Crosshair";
            Icon = AppIcon.MakeIcon(32);
            DoubleBuffered = true;
            Font = Theme.Normal;

            input = new InputRow("Je naam", delegate { Done(); });
            input.BackColor = Theme.Window;
            input.SetBounds(30, 128, 320, 36);
            input.Value = current;
            Controls.Add(input);

            error = new Label();
            error.ForeColor = Theme.Danger; error.BackColor = Theme.Window; error.Font = Theme.Tiny;
            error.SetBounds(30, 166, 320, 18);
            Controls.Add(error);

            PrimaryButton ok = new PrimaryButton("Verder", delegate { Done(); });
            ok.BackColor = Theme.Window;
            ok.SetBounds(30, 192, 320, 38);
            Controls.Add(ok);
        }

        void Done()
        {
            string n = (input.Value ?? "").Trim();
            if (n.Length == 0) { error.Text = "Vul eerst je naam in."; return; }
            if (n.Length > 16) n = n.Substring(0, 16);
            Result = n;
            DialogResult = DialogResult.OK;
            Close();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }

        protected override void OnShown(EventArgs e) { base.OnShown(e); Activate(); input.Focus(); input.FocusBox(); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(logo, 30, 24, 42, 42);
            Theme.Label(g, "Welkom!", Theme.PageFont, Theme.Text, new Rectangle(84, 18, 280, 34), TextFormatFlags.Left);
            Theme.Label(g, "Welkom bij Custom Crosshair", Theme.Normal, Theme.Dim, new Rectangle(86, 50, 280, 20), TextFormatFlags.Left);
            Theme.Label(g, "Hoe heet je?", Theme.Bold, Theme.Text, new Rectangle(30, 96, 320, 24), TextFormatFlags.Left);
            using (Pen p = new Pen(Theme.Border)) g.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero); }
        }
    }

    public class UpdateNotice : Form
    {
        int left = 10;
        Bitmap logo = AppIcon.MakeBitmap(112);
        Timer tm;
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        public UpdateNotice()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(420, 210);
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = true;
            Text = "Custom Crosshair - update";
            Icon = AppIcon.MakeIcon(32);
            DoubleBuffered = true;
            PrimaryButton now = new PrimaryButton("Nu updaten", delegate { Done(); });
            now.BackColor = Theme.Window;
            now.SetBounds(28, 150, 364, 38);
            Controls.Add(now);
            tm = new Timer();
            tm.Interval = 1000;
            tm.Tick += delegate { left--; Invalidate(); if (left <= 0) Done(); };
            tm.Start();
        }
        void Done() { tm.Stop(); DialogResult = DialogResult.OK; Close(); }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (DialogResult != DialogResult.OK) { e.Cancel = true; return; }
            base.OnFormClosing(e);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen p = new Pen(Theme.Border)) g.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(logo, 28, 26, 40, 40);
            Theme.Label(g, "Update beschikbaar", Theme.Title, Theme.Text, new Rectangle(80, 26, 320, 24), TextFormatFlags.Left);
            Theme.Label(g, "Custom Crosshair", Theme.Tiny, Theme.Dim, new Rectangle(80, 48, 320, 18), TextFormatFlags.Left);
            TextRenderer.DrawText(g, "Er is een nieuwe versie. De app wordt over " + left + (left == 1 ? " seconde" : " seconden") + " bijgewerkt en daarna vanzelf opnieuw gestart.",
                Theme.Normal, new Rectangle(28, 82, 364, 44), Theme.Dim, TextFormatFlags.WordBreak | TextFormatFlags.Left);
            RectangleF track = new RectangleF(28, 132, 364, 4);
            Theme.FillRound(g, Theme.Track, track, 2);
            Theme.FillRound(g, Theme.Accent, new RectangleF(track.X, track.Y, track.Width * (10 - left) / 10f, track.Height), 2);
        }
    }

    public class SplashForm : Form
    {
        Bitmap logo = AppIcon.MakeBitmap(160);
        public float Progress = 0f;
        public string Status = "Laden...";
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        public SplashForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(440, 280);
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = true;
            Text = "Custom Crosshair";
            Icon = AppIcon.MakeIcon(32);
            DoubleBuffered = true;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int W = ClientSize.Width;
            using (Pen p = new Pen(Theme.Border)) g.DrawRectangle(p, 0, 0, W - 1, ClientSize.Height - 1);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(logo, W / 2 - 40, 36, 80, 80);
            Theme.Label(g, "Custom Crosshair", Theme.PageFont, Theme.Text, new Rectangle(0, 128, W, 36), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, Status, Theme.Normal, Theme.Dim, new Rectangle(0, 166, W, 22), TextFormatFlags.HorizontalCenter);
            RectangleF track = new RectangleF(90, 212, W - 180, 6);
            Theme.FillRound(g, Theme.Track, track, 3);
            if (Progress > 0) Theme.FillRound(g, Theme.Accent, new RectangleF(track.X, track.Y, track.Width * Math.Min(1f, Progress), track.Height), 3);
            Theme.Label(g, "versie " + Updater.Version, Theme.Tiny, Theme.Faint, new Rectangle(0, 244, W, 18), TextFormatFlags.HorizontalCenter);
        }
    }

    public class TourDim : Form
    {
        public Rectangle Hole = Rectangle.Empty;
        static readonly Color Key = Color.FromArgb(255, 0, 255);
        public TourDim()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.Black;
            TransparencyKey = Key;
            Opacity = 0.6;
            DoubleBuffered = true;
            TopMost = true;
        }
        protected override bool ShowWithoutActivation { get { return true; } }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Hole.IsEmpty) using (SolidBrush b = new SolidBrush(Key)) e.Graphics.FillRectangle(b, Hole);
        }
    }

    public class TourBubble : Form
    {
        string title, text, step;
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        public TourBubble(string title, string text, string step, bool last, Action next, Action skip)
        {
            this.title = title; this.text = text; this.step = step;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(320, 178);
            BackColor = Theme.Card;
            TopMost = true;
            DoubleBuffered = true;
            PrimaryButton nb = new PrimaryButton(last ? "Klaar" : "Volgende", next);
            nb.BackColor = Theme.Card;
            nb.SetBounds(176, 128, 128, 34);
            Controls.Add(nb);
            if (!last)
            {
                LinkRow sk = new LinkRow("Overslaan", skip);
                sk.BackColor = Theme.Card;
                sk.SetBounds(96, 132, 76, 26);
                Controls.Add(sk);
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen p = new Pen(Theme.Accent, 2)) g.DrawRectangle(p, 1, 1, ClientSize.Width - 3, ClientSize.Height - 3);
            Theme.Label(g, title, Theme.Title, Theme.Text, new Rectangle(18, 14, 284, 26), TextFormatFlags.Left);
            TextRenderer.DrawText(g, text, Theme.Normal, new Rectangle(18, 44, 286, 78), Theme.Dim, TextFormatFlags.WordBreak | TextFormatFlags.Left);
            Theme.Label(g, step, Theme.Tiny, Theme.Faint, new Rectangle(18, 132, 70, 26), TextFormatFlags.Left);
        }
    }

    public class WhatsNewForm : Form
    {
        Bitmap logo = AppIcon.MakeBitmap(112);
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        public WhatsNewForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(400, 150 + Updater.Notes.Length * 30);
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = false;
            DoubleBuffered = true;
            PrimaryButton ok = new PrimaryButton("Top!", delegate { Close(); });
            ok.BackColor = Theme.Window;
            ok.SetBounds(24, ClientSize.Height - 58, ClientSize.Width - 48, 36);
            Controls.Add(ok);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen p = new Pen(Theme.Border)) g.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(logo, 24, 22, 36, 36);
            Theme.Label(g, "Wat is er nieuw", Theme.PageFont, Theme.Text, new Rectangle(70, 20, 300, 34), TextFormatFlags.Left);
            Theme.Label(g, "Custom Crosshair is bijgewerkt", Theme.Tiny, Theme.Dim, new Rectangle(72, 50, 300, 16), TextFormatFlags.Left);
            int y = 84;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            foreach (string n in Updater.Notes)
            {
                using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillEllipse(b, 28, y + 8, 7, 7);
                Theme.Label(g, n, Theme.Normal, Theme.Text, new Rectangle(46, y, ClientSize.Width - 70, 24), TextFormatFlags.Left);
                y += 30;
            }
        }
    }

    // ------------------------------------------------------------------ het menu
    public class MenuForm : Form
    {
        Settings s; Overlay overlay;
        int page = 0, tab = 0;
        const int SideW = 190;
        List<NavItem> navs = new List<NavItem>();
        List<Control> tabCtls = new List<Control>();
        Panel content;
        Bitmap logo = AppIcon.MakeBitmap(112);

        [DllImport("user32.dll")] static extern bool ReleaseCapture();
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);

        static readonly Color[] Presets = new Color[] {
            Color.FromArgb(0, 255, 0), Color.FromArgb(255, 40, 40), Color.FromArgb(0, 230, 255),
            Color.FromArgb(255, 230, 0), Color.FromArgb(255, 255, 255), Color.FromArgb(255, 80, 200),
            Color.FromArgb(255, 140, 0)
        };

        static readonly Color[] AccentPresets = new Color[] {
            Color.FromArgb(225, 225, 230), Color.FromArgb(88, 101, 242), Color.FromArgb(155, 89, 230),
            Color.FromArgb(40, 200, 120), Color.FromArgb(236, 72, 153), Color.FromArgb(245, 140, 40),
            Color.FromArgb(230, 70, 70)
        };

        public MenuForm(Settings settings, Overlay ov)
        {
            s = settings; overlay = ov;
            FormBorderStyle = FormBorderStyle.None;
            ClientSize = new Size(840, 520);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = true;
            Text = "Custom Crosshair";
            Icon = AppIcon.MakeIcon(32);
            DoubleBuffered = true;
            Opacity = Math.Max(70, Math.Min(100, s.MenuOpacity)) / 100.0;
            Font = Theme.Normal;

            AddNav("X", "Crosshair", 0, 0);
            AddNav("\uE790", "Kleur", 1, 1);
            AddNav("\uE8AB", "Positie", 6, 2);
            AddNav("\uE70F", "Eigen", 7, 3);
            AddNav("\uE8FD", "Profielen", 2, 4);
            AddNav("\uE713", "Instellingen", 3, 5);
            AddNav("\uE895", "Updates", 4, 6);

            close = new CloseButton(delegate { Toggle(); });
            close.Location = new Point(ClientSize.Width - 46, 12);
            Controls.Add(close);

            content = new Panel();
            content.Left = SideW + 14;
            content.Top = 60;
            content.Width = ClientSize.Width - SideW - 28;
            content.Height = ClientSize.Height - 72;
            content.BackColor = Theme.Window;
            content.AutoScroll = true;
            Controls.Add(content);

            ApplyLayout();
            ShowPage(0);

            Timer ft = new Timer();
            ft.Interval = 30000;
            ft.Tick += delegate { if (Visible && page == 8) FiveM.Refresh(this, delegate { content.Invalidate(true); }); };
            ft.Start();
        }

        CloseButton close;
        int TabX { get { return Theme.Glass ? 16 : SideW + 14; } }
        int TabY { get { return Theme.Glass ? 68 : 12; } }

        JumpBar jumpBar;
        NewsButton newsBtn;
        Panel contentHost;

        void ApplyLayout()
        {
            ClientSize = new Size(1000, 600);
            foreach (NavItem n in navs)
            {
                n.Visible = true;
                n.BackColor = Theme.Sidebar;
                n.SetBounds(12, 108 + n.SideTop * 42, 176, 36);
            }
            if (newsBtn == null)
            {
                newsBtn = new NewsButton(() => s.NewsSeenVersion != Updater.Version, delegate
                {
                    s.NewsSeenVersion = Updater.Version;
                    s.Save();
                    newsBtn.Invalidate();
                    BeginInvoke(new Action(ShowWhatsNew));
                });
                Controls.Add(newsBtn);
            }
            newsBtn.BackColor = Theme.Sidebar;
            newsBtn.SetBounds(12, 62, 176, 32);
            if (contentHost == null)
            {
                contentHost = new Panel();
                Controls.Add(contentHost);
                contentHost.Controls.Add(content);
            }
            if (leftPanel == null)
            {
                leftPanel = new Panel();
                Controls.Add(leftPanel);
            }
            if (jumpBar == null)
            {
                jumpBar = new JumpBar(new string[0], delegate(int i) { });
                Controls.Add(jumpBar);
            }
            jumpBar.Visible = false;
            leftPanel.SetBounds(670, 56, 314, ClientSize.Height - 72);
            leftPanel.BackColor = Theme.Window;
            contentHost.SetBounds(220, 92, 434, ClientSize.Height - 104);
            contentHost.BackColor = Theme.Window;
            content.SetBounds(0, 0, 434 + SystemInformation.VerticalScrollBarWidth, ClientSize.Height - 104);
            content.BackColor = Theme.Window;
            close.BackColor = Theme.Window;
            close.Location = new Point(ClientSize.Width - 44, 12);
        }

        void AddNav(string glyph, string caption, int index, int top)
        {
            NavItem n = new NavItem(glyph, caption, delegate { ShowPage(index); });
            n.PageIndex = index;
            n.SideTop = top;
            n.Location = new Point(10, top);
            Controls.Add(n);
            navs.Add(n);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int W = ClientSize.Width, H = ClientSize.Height;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            PaintSidebarLayout(g, W, H);
            return;
            using (SolidBrush b = new SolidBrush(Theme.Sidebar)) g.FillRectangle(b, 0, 0, SideW, H);
            using (Pen p = new Pen(Theme.Border)) g.DrawLine(p, SideW, 0, SideW, H);

            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            if (Theme.Glass) { PaintGlass(g, W, H); return; }
            if (!Theme.Lounge) { PaintClean(g, W, H); return; }
            g.DrawImage(logo, 16, 18, 34, 34);
            Theme.Label(g, "CUSTOM", Theme.Heavy, Theme.Text, new Rectangle(56, 12, 132, 24), TextFormatFlags.Left);
            Theme.Label(g, "CROSSHAIR", Theme.Heavy, Theme.Text, new Rectangle(56, 32, 132, 24), TextFormatFlags.Left);
            Theme.Bar(g, Theme.Text, 58, 60, 44);
            using (Pen p = new Pen(Color.FromArgb(60, 60, 68))) { g.DrawLine(p, 108, 61, 172, 61); }

            Theme.Label(g, Theme.Caps("Algemeen"), Theme.Spaced, Theme.Faint, new Rectangle(20, 82, 160, 18), TextFormatFlags.Left);
            Theme.Label(g, Theme.Caps("Overig"), Theme.Spaced, Theme.Faint, new Rectangle(20, 234, 160, 18), TextFormatFlags.Left);

            RectangleF card = new RectangleF(10, H - 64, 170, 50);
            Theme.FillRound(g, Color.FromArgb(22, 22, 26), card, 5);
            Theme.DrawRound(g, Theme.Border, card, 5);
            using (SolidBrush b = new SolidBrush(Color.FromArgb(232, 232, 236))) g.FillEllipse(b, 20, H - 53, 28, 28);
            string uname = string.IsNullOrEmpty(s.UserName) ? "Speler" : s.UserName;
            Theme.Label(g, uname.Substring(0, 1).ToUpper(), Theme.HeavySmall, Color.FromArgb(20, 20, 24), new Rectangle(20, H - 53, 28, 28), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, uname.ToUpper(), Theme.HeavySmall, Theme.Text, new Rectangle(58, H - 58, 118, 22), TextFormatFlags.Left);
            Theme.Label(g, "profiel: " + s.ActiveProfile, Theme.Tiny, Theme.Dim, new Rectangle(58, H - 38, 118, 18), TextFormatFlags.Left);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            int x0 = W - 290, top = 10, bot = 46, right = W - 56;
            using (SolidBrush b = new SolidBrush(Color.FromArgb(112, 112, 120)))
                g.FillPolygon(b, new PointF[] { new PointF(x0 - 2, top), new PointF(x0 + 8, top), new PointF(x0 - 10, bot), new PointF(x0 - 20, bot) });
            using (LinearGradientBrush b = new LinearGradientBrush(new Rectangle(x0 - 10, top, right - x0 + 10, bot - top), Color.FromArgb(210, 210, 216), Color.FromArgb(244, 244, 247), 0f))
                g.FillPolygon(b, new PointF[] { new PointF(x0 + 14, top), new PointF(right, top), new PointF(right, bot), new PointF(x0 - 4, bot) });
            using (Pen p = new Pen(Color.FromArgb(175, 175, 184)))
            {
                g.DrawLine(p, right - 90, top + 6, right - 10, top + 6);
                g.DrawLine(p, right - 70, top + 10, right - 10, top + 10);
            }
            Theme.Label(g, Theme.Caps(Theme.KeyName(s.MenuKey) + " = menu"), Theme.Spaced, Color.FromArgb(34, 34, 40),
                        new Rectangle(x0 + 10, top + 8, right - x0 - 20, bot - top - 8), TextFormatFlags.HorizontalCenter);
        }

        void PaintSidebarLayout(Graphics g, int W, int H)
        {
            using (SolidBrush sb = new SolidBrush(Theme.Sidebar)) g.FillRectangle(sb, 0, 0, 200, H);
            using (Pen p = new Pen(Theme.Border)) g.DrawLine(p, 200, 0, 200, H);
            g.DrawImage(logo, 18, 20, 30, 30);
            Theme.Label(g, "Custom Crosshair", Theme.Bold, Theme.Text, new Rectangle(56, 18, 140, 20), TextFormatFlags.Left);
            Theme.Label(g, Theme.KeyName(s.MenuKey) + " = menu", Theme.Tiny, Theme.Dim, new Rectangle(56, 37, 140, 16), TextFormatFlags.Left);

            string uname = string.IsNullOrEmpty(s.UserName) ? "Speler" : s.UserName;
            RectangleF chip = new RectangleF(12, H - 62, 176, 48);
            Theme.FillRound(g, Theme.Card, chip, 24);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillEllipse(b, 18, H - 56, 36, 36);
            Theme.Label(g, uname.Substring(0, 1).ToUpper(), Theme.Bold, Theme.AccentText, new Rectangle(18, H - 56, 36, 36), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, uname, Theme.Bold, Theme.Text, new Rectangle(62, H - 58, 120, 20), TextFormatFlags.Left);
            Theme.Label(g, s.ActiveProfile, Theme.Tiny, Theme.Dim, new Rectangle(62, H - 38, 120, 16), TextFormatFlags.Left);

            Theme.Label(g, PageTitle(page), Theme.PageFont, Theme.Text, new Rectangle(226, 18, 420, 36), TextFormatFlags.Left);
            Theme.Label(g, PageSub(page), Theme.Normal, Theme.Dim, new Rectangle(228, 54, 430, 22), TextFormatFlags.Left);
        }

        void PaintDashboard(Graphics g, int W, int H)
        {
            using (SolidBrush hb = new SolidBrush(Theme.Sidebar)) g.FillRectangle(hb, 0, 0, W, 60);
            using (Pen p = new Pen(Theme.Border)) g.DrawLine(p, 0, 60, W, 60);
            g.DrawImage(logo, 18, 14, 32, 32);
            if (Theme.Lounge)
            {
                Theme.Label(g, "CUSTOM CROSSHAIR", Theme.Heavy, Theme.Text, new Rectangle(60, 8, 300, 26), TextFormatFlags.Left);
                Theme.Bar(g, Theme.Text, 62, 36, 40);
            }
            else Theme.Label(g, "Custom Crosshair", Theme.Title, Theme.Text, new Rectangle(60, 9, 300, 24), TextFormatFlags.Left);
            Theme.Label(g, Theme.KeyName(s.MenuKey) + " = menu openen/sluiten", Theme.Tiny, Theme.Dim, new Rectangle(Theme.Lounge ? 110 : 60, 32, 300, 18), TextFormatFlags.Left);

            string uname = string.IsNullOrEmpty(s.UserName) ? "Speler" : s.UserName;
            int ux = W - 290;
            RectangleF chip = new RectangleF(ux, 13, 226, 34);
            Theme.FillRound(g, Theme.Card, chip, 17);
            Theme.DrawRound(g, Theme.Border, chip, 17);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillEllipse(b, ux + 5, 18, 24, 24);
            Theme.Label(g, uname.Substring(0, 1).ToUpper(), Theme.Bold, Theme.AccentText, new Rectangle(ux + 5, 18, 24, 24), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, uname, Theme.Bold, Theme.Text, new Rectangle(ux + 36, 13, 100, 34), TextFormatFlags.Left);
            Theme.Label(g, s.ActiveProfile, Theme.Tiny, Theme.Dim, new Rectangle(ux + 136, 13, 82, 34), TextFormatFlags.Right);
        }

        void PaintGlass(Graphics g, int W, int H)
        {
            using (SolidBrush hb = new SolidBrush(Theme.Sidebar)) g.FillRectangle(hb, 0, 0, W, 58);
            using (Pen p = new Pen(Theme.Border)) g.DrawLine(p, 0, 58, W, 58);
            using (Pen p = new Pen(Color.FromArgb(30, 255, 255, 255))) g.DrawLine(p, 0, 0, W, 0);
            g.DrawImage(logo, 16, 13, 32, 32);
            Theme.Label(g, "Custom Crosshair", Theme.Title, Theme.Text, new Rectangle(56, 10, 145, 22), TextFormatFlags.Left);
            Theme.Label(g, Theme.KeyName(s.MenuKey) + " = menu", Theme.Tiny, Theme.Dim, new Rectangle(56, 31, 145, 16), TextFormatFlags.Left);

            // gebruiker rechts naast de tabbladen
            string uname = string.IsNullOrEmpty(s.UserName) ? "Speler" : s.UserName;
            int ux = W - 220;
            RectangleF chip = new RectangleF(ux, 68, 204, 34);
            Theme.FillRound(g, Theme.Card, chip, 17);
            Theme.DrawRound(g, Theme.Border, chip, 17);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillEllipse(b, ux + 5, 73, 24, 24);
            Theme.Label(g, uname.Substring(0, 1).ToUpper(), Theme.Bold, Theme.AccentText, new Rectangle(ux + 5, 73, 24, 24), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, uname, Theme.Bold, Theme.Text, new Rectangle(ux + 36, 68, 90, 34), TextFormatFlags.Left);
            Theme.Label(g, s.ActiveProfile, Theme.Tiny, Theme.Dim, new Rectangle(ux + 126, 68, 72, 34), TextFormatFlags.Right);
        }

        void PaintClean(Graphics g, int W, int H)
        {
            g.DrawImage(logo, 18, 20, 30, 30);
            Theme.Label(g, "Custom Crosshair", Theme.Title, Theme.Text, new Rectangle(56, 16, 130, 22), TextFormatFlags.Left);
            Theme.Label(g, Theme.KeyName(s.MenuKey) + " = menu", Theme.Tiny, Theme.Dim, new Rectangle(56, 37, 130, 16), TextFormatFlags.Left);

            Theme.Label(g, "ALGEMEEN", Theme.Small, Theme.Faint, new Rectangle(22, 82, 150, 18), TextFormatFlags.Left);
            Theme.Label(g, "OVERIG", Theme.Small, Theme.Faint, new Rectangle(22, 234, 150, 18), TextFormatFlags.Left);

            using (Pen p = new Pen(Theme.Border)) g.DrawLine(p, 14, H - 72, SideW - 14, H - 72);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using (SolidBrush b = new SolidBrush(Theme.Accent)) g.FillEllipse(b, 18, H - 56, 32, 32);
            string uname = string.IsNullOrEmpty(s.UserName) ? "Speler" : s.UserName;
            Theme.Label(g, uname.Substring(0, 1).ToUpper(), Theme.Bold, Theme.AccentText, new Rectangle(18, H - 56, 32, 32), TextFormatFlags.HorizontalCenter);
            Theme.Label(g, uname, Theme.Bold, Theme.Text, new Rectangle(60, H - 58, 120, 20), TextFormatFlags.Left);
            Theme.Label(g, s.ActiveProfile, Theme.Tiny, Theme.Dim, new Rectangle(60, H - 39, 120, 16), TextFormatFlags.Left);
        }

        public void ApplyTheme()
        {
            Theme.Apply(s.MenuTheme, s.Accent);
            Opacity = Math.Max(70, Math.Min(100, s.MenuOpacity)) / 100.0;
            BackColor = Theme.Window;
            content.BackColor = Theme.Window;
            ApplyLayout();
            jumpBar.Invalidate();
            foreach (NavItem n in navs) n.Invalidate();
            BuildTabs();
            BuildContent();
            Invalidate(true);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero);
            }
        }

        void ShowPage(int p)
        {
            page = p; tab = 0;
            if (p == 8) FiveM.Refresh(this, delegate { content.Invalidate(true); });
            foreach (NavItem n in navs) { n.Selected = n.PageIndex == p; n.Invalidate(); }
            BuildTabs();
            BuildContent();
        }

        void BuildTabs()
        {
            foreach (Control c in tabCtls) c.Dispose();
            tabCtls.Clear();
            if (leftPanel != null) return;
            string[] names, glyphs;
            if (page == 0) { names = new string[] { "Stijl", "Vorm", "Positie", "Eigen" }; glyphs = new string[] { "\uE8A9", "\uE8B3", "\uE8AB", "\uE70F" }; }
            else if (page == 1) { names = new string[] { "Kleur" }; glyphs = new string[] { "\uE790" }; }
            else if (page == 2) { names = new string[] { "Profielen" }; glyphs = new string[] { "\uE8FD" }; }
            else if (page == 4) { names = new string[] { "Updates" }; glyphs = new string[] { "\uE895" }; }
            else if (page == 5) { names = new string[] { "Uiterlijk" }; glyphs = new string[] { "\uE771" }; }
            else { names = new string[] { "Algemeen" }; glyphs = new string[] { "\uE713" }; }
            int x = TabX;
            for (int i = 0; i < names.Length; i++)
            {
                int idx = i;
                TabButton tb = new TabButton(glyphs[i], names[i], i == tab, delegate
                {
                    tab = idx;
                    BeginInvoke(new Action(delegate { BuildTabs(); BuildContent(); }));
                });
                tb.Location = new Point(x, TabY);
                x += tb.Width + 6;
                Controls.Add(tb);
                tabCtls.Add(tb);
            }
        }

        public void InvalidatePreviews() { InvalidateAnimated(content); if (leftPanel != null) InvalidateAnimated(leftPanel); }

        void InvalidateAnimated(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is PreviewBox || c is StyleRow || c is ColorRow || c is SwatchRow) c.Invalidate();
                if (c.HasChildren) InvalidateAnimated(c);
            }
        }

        void CopyCode()
        {
            try
            {
                Clipboard.SetText(ShareCode.Make(s));
                MessageBox.Show(this, s.Style == Settings.StyleImage
                    ? "Je code is gekopieerd. Let op: een eigen plaatje past niet in een code, je vriend moet het plaatje zelf ook kiezen."
                    : "Je crosshair-code is gekopieerd. Plak hem in Discord of waar je maar wilt.", "Custom Crosshair");
            }
            catch { MessageBox.Show(this, "Kopieren lukte niet, probeer het nog eens.", "Custom Crosshair"); }
        }

        void UseCode(string code)
        {
            if (!ShareCode.Apply(code, s)) { MessageBox.Show(this, "Dit is geen geldige crosshair-code. Een code begint met BC-", "Custom Crosshair"); return; }
            Changed();
            Rebuild();
        }

        public void RefreshAll()
        {
            if (Visible && IsHandleCreated) BeginInvoke(new Action(BuildContent));
            Invalidate();
        }

        void CreateProfile(string raw)
        {
            string name = Profiles.Clean(raw);
            if (name.Length == 0) { MessageBox.Show(this, "Typ eerst een naam voor het profiel.", "Custom Crosshair"); return; }
            if (Profiles.Exists(name)) { MessageBox.Show(this, "Er bestaat al een profiel met de naam '" + name + "'.", "Custom Crosshair"); return; }
            Profiles.Save(name, s);
            s.ActiveProfile = name;
            Changed();
            Rebuild();
        }

        void Changed()
        {
            s.Save();
            overlay.ApplySettings();
            content.Invalidate(true);
            if (leftPanel != null) leftPanel.Invalidate(true);
            Invalidate();
        }

        void Rebuild() { BeginInvoke(new Action(BuildContent)); }

        static readonly string[] SectionNames = new string[] { "Stijl", "Vorm", "Kleur", "Positie", "Eigen", "Profielen", "Instellingen", "Uiterlijk", "Updates" };
        static readonly int[][] SectionPages = new int[][] {
            new int[] {0,0}, new int[] {0,1}, new int[] {1,0}, new int[] {0,2}, new int[] {0,3},
            new int[] {2,0}, new int[] {3,0}, new int[] {5,0}, new int[] {4,0} };
        List<SectionHeader> sections = new List<SectionHeader>();

        void JumpTo(int i)
        {
            if (i < 0 || i >= sections.Count) return;
            int abs = sections[i].Top - content.AutoScrollPosition.Y;
            content.AutoScrollPosition = new Point(0, abs);
        }

        bool showAllStyles = false;
        static readonly int[] PageIds = new int[] { 0, 1, 6, 7, 2, 3, 4 };
        string PageTitle(int p)
        {
            switch (p)
            {
                case 0: return "Crosshair";
                case 1: return "Kleur";
                case 6: return "Positie";
                case 7: return "Eigen";
                case 2: return "Profielen";
                case 3: return "Instellingen";
                case 4: return "Updates";
                case 8: return "FiveM";
                default: return "";
            }
        }
        string PageSub(int p)
        {
            switch (p)
            {
                case 0: return "Kies een stijl en pas de vorm aan";
                case 1: return "Kies de kleur van je crosshair";
                case 6: return "Verschuif je crosshair als het midden van je game anders zit";
                case 7: return "Teken je eigen crosshair of gebruik een plaatje";
                case 2: return "Bewaar je crosshairs en deel ze met vrienden";
                case 3: return "Toetsen, schermen en hoe de app werkt";
                case 4: return "Houd de app up-to-date";
                case 8: return "Je favoriete servers, live spelers en snel joinen";
                default: return "";
            }
        }

        void BuildContent()
        {
            content.SuspendLayout();
            content.AutoScrollPosition = new Point(0, 0);
            while (content.Controls.Count > 0) content.Controls[0].Dispose();
            sections.Clear();
            int colW = content.Width - SystemInformation.VerticalScrollBarWidth - 4;
            List<Card> cards = new List<Card>();

            if (page == 0)
            {
                Card st = new Card("X", "Stijl", colW);
                if (showAllStyles)
                {
                    st.Add(new StyleGrid(s, v => { s.Style = v; Changed(); Rebuild(); }));
                    st.Add(new LinkRow("\u2039  Minder tonen", delegate { showAllStyles = false; Rebuild(); }));
                }
                else
                {
                    if (s.Style == Settings.StyleImage || s.Style == Settings.StylePixels)
                        st.Add(new InfoRow("Je gebruikt nu je eigen " + (s.Style == Settings.StyleImage ? "plaatje" : "tekening") + ". Kies hier een stijl om terug te gaan.", 30));
                    int[] quick = new int[] { 0, 2, 1, 5, 6, 8 };
                    if (s.Style != Settings.StyleImage && s.Style != Settings.StylePixels && Array.IndexOf(quick, s.Style) < 0) quick[5] = s.Style;
                    st.Add(new StyleGrid(s, v => { s.Style = v; Changed(); Rebuild(); }, quick));
                    st.Add(new LinkRow("Alle stijlen bekijken  \u203A", delegate { showAllStyles = true; Rebuild(); }));
                }
                cards.Add(st);

                Card vm = new Card("\uE740", "Vorm", colW);
                vm.Add(new SliderRow("\uE740", "Lengte", 1, 40, s.Length, v => v + " px", v => { s.Length = v; Changed(); }));
                vm.Add(new SliderRow("\uE7C3", "Dikte", 1, 10, s.Thickness, v => v + " px", v => { s.Thickness = v; Changed(); }));
                vm.Add(new SliderRow("\uE73F", "Afstand tot midden", 0, 30, s.Gap, v => v + " px", v => { s.Gap = v; Changed(); }));
                vm.Add(new SliderRow("\uE71E", "Puntgrootte", 1, 12, s.DotSize, v => v + " px", v => { s.DotSize = v; Changed(); }));
                vm.Add(new SliderRow("\uE7B3", "Zichtbaarheid", 10, 100, s.Opacity, v => v + "%", v => { s.Opacity = v; Changed(); }));
                cards.Add(vm);
            }
            else if (page == 8)
            {
                Card sv = new Card("\uE7FC", "Mijn servers", colW);
                if (FiveM.Servers.Count == 0)
                    sv.Add(new InfoRow("Nog geen servers. Voeg hieronder je eerste server toe.", 22));
                foreach (FServer f0 in FiveM.Servers)
                {
                    FServer f = f0;
                    ServerRow row = null;
                    row = sv.Add(new ServerRow(f,
                        delegate
                        {
                            if (s.FivemAutoProfile && f.Profile.Length > 0 && Profiles.Exists(f.Profile)) Program.SwitchProfile(f.Profile);
                            FiveM.Join(f);
                        },
                        delegate
                        {
                            if (MessageBox.Show(this, "'" + f.Name + "' uit je lijst halen?", "Custom Crosshair", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            { FiveM.Servers.Remove(f); FiveM.Save(); Rebuild(); }
                        },
                        delegate
                        {
                            ContextMenuStrip cms = new ContextMenuStrip();
                            cms.Font = Theme.Normal;
                            cms.Items.Add("Geen profiel", null, delegate { f.Profile = ""; FiveM.Save(); row.Invalidate(); });
                            cms.Items.Add(new ToolStripSeparator());
                            foreach (string pn0 in Profiles.Names())
                            {
                                string pn = pn0;
                                ToolStripItem it = cms.Items.Add(pn, null, delegate { f.Profile = pn; FiveM.Save(); row.Invalidate(); });
                                if (pn == f.Profile) ((ToolStripMenuItem)it).Checked = true;
                            }
                            cms.Closed += delegate { BeginInvoke(new Action(cms.Dispose)); };
                            cms.Show(row, new Point(row.Width - 300, 64));
                        }));
                }
                if (FiveM.Servers.Count > 0)
                    sv.Add(new LinkRow("Vernieuwen  \u21BB", delegate { FiveM.Refresh(this, delegate { content.Invalidate(true); }); }));
                cards.Add(sv);

                Card add = new Card("\uE710", "Server toevoegen", colW);
                add.Add(new InfoRow("Plak het connect-adres van een server, bijv. connect 45.8.187.84:30120, of een join-link zoals cfx.re/join/abc123.", 32));
                InputRow inp = null;
                Action addIt = delegate
                {
                    string code = FiveM.ParseCode(inp.Value);
                    if (code == null) { MessageBox.Show(this, "Dit adres snap ik niet. Gebruik bijv. connect 45.8.187.84:30120 of cfx.re/join/abc123.", "Custom Crosshair"); return; }
                    foreach (FServer x in FiveM.Servers) if (x.Code == code) { MessageBox.Show(this, "Deze server staat al in je lijst.", "Custom Crosshair"); return; }
                    FServer nf = new FServer(); nf.Code = code; nf.Name = FiveM.IsDirect(code) ? code : "cfx.re/join/" + code;
                    FiveM.Servers.Add(nf); FiveM.Save();
                    Rebuild();
                    FiveM.Refresh(this, delegate { content.Invalidate(true); });
                };
                inp = add.Add(new InputRow("connect 1.2.3.4:30120", addIt));
                add.Add(new PrimaryButton("Toevoegen", addIt));
                add.Add(new ToggleRow("\uE8FD", "Crosshair-profiel automatisch wisselen bij joinen", s.FivemAutoProfile, v => { s.FivemAutoProfile = v; s.Save(); }));
                add.Add(new ButtonRow("\uE74D", "FiveM-cache legen", false, delegate { FiveM.ClearCache(this); }));
                cards.Add(add);
            }
            else
            {
                int pg = page == 6 || page == 7 ? 0 : page;
                int tb = page == 6 ? 2 : (page == 7 ? 3 : 0);
                Card[] built = BuildCards(pg, tb, colW);
                for (int i = 0; i < built.Length; i++)
                {
                    Card c = built[i];
                    if (c == null) continue;
                    if ((page == 1 || page == 6) && i == 1) { c.Dispose(); continue; }
                    c.RemovePreviews();
                    if (c.IsEmpty) { c.Dispose(); continue; }
                    cards.Add(c);
                }
            }

            int y = 0;
            foreach (Card c in cards)
            {
                c.Location = new Point(0, y);
                content.Controls.Add(c);
                y = c.Bottom + 12;
            }
            Panel spacer = new Panel();
            spacer.SetBounds(0, y, 10, 12);
            spacer.BackColor = Theme.Window;
            content.Controls.Add(spacer);
            content.ResumeLayout();
            BuildLeft();
            Invalidate();
        }

        Card[] BuildCards(int page, int tab, int colW)
        {
            int cw = colW;
            Card L = null, R = null, C = null;
            bool stacked = false;

            if (page == 0 && tab == 0)
            {
                stacked = true;
                int sw = content.Width - SystemInformation.VerticalScrollBarWidth - 4;
                L = new Card("X", "Kies je crosshair", sw);
                if (s.Style == Settings.StyleImage || s.Style == Settings.StylePixels)
                    L.Add(new InfoRow("Je gebruikt nu je eigen " + (s.Style == Settings.StyleImage ? "plaatje" : "tekening") + " (tabblad Eigen). Kies hieronder een stijl om terug te gaan.", 20));
                L.Add(new StyleGrid(s, v => { s.Style = v; Changed(); Rebuild(); }));
            }
            else if (page == 0 && tab == 1)
            {
                L = new Card("X", "Crosshair", cw);
                L.Add(new ToggleRow("\uE7E8", "Crosshair aan", s.Enabled, v => { s.Enabled = v; Changed(); }));
                L.Add(new ToggleRow("\uE739", "Zwarte rand", s.Outline, v => { s.Outline = v; Changed(); }));
                L.Add(new SliderRow("\uE7B3", "Zichtbaarheid", 10, 100, s.Opacity, v => v + "%", v => { s.Opacity = v; Changed(); }));

                R = new Card("\uE740", "Afmetingen", cw);
                R.Add(new SliderRow("\uE740", "Lengte", 1, 40, s.Length, v => v + " px", v => { s.Length = v; Changed(); }));
                R.Add(new SliderRow("\uE7C3", "Dikte", 1, 10, s.Thickness, v => v + " px", v => { s.Thickness = v; Changed(); }));
                R.Add(new SliderRow("\uE73F", "Afstand tot midden", 0, 30, s.Gap, v => v + " px", v => { s.Gap = v; Changed(); }));
                R.Add(new SliderRow("\uE71E", "Puntgrootte", 1, 12, s.DotSize, v => v + " px", v => { s.DotSize = v; Changed(); }));
            }
            else if (page == 0 && tab == 2)
            {
                L = new Card("\uE8AB", "Positie", cw);
                L.Add(new SliderRow("\uE8AB", "Links / rechts", -60, 60, s.OffsetX, v => v + " px", v => { s.OffsetX = v; Changed(); }));
                L.Add(new SliderRow("\uE8AB", "Omhoog / omlaag", -60, 60, s.OffsetY, v => v + " px", v => { s.OffsetY = v; Changed(); }));
                L.Add(new ButtonRow("\uE72C", "Terug naar het midden", false, delegate { s.OffsetX = 0; s.OffsetY = 0; Changed(); Rebuild(); }));

                R = new Card("\uE7B3", "Voorbeeld", cw);
                R.Add(new PreviewBox(s));
            }
            else if (page == 0)
            {
                L = new Card("\uE70F", "Zelf tekenen", cw);
                L.Add(new InfoRow("Linkermuis = tekenen, rechtermuis = gummen. Het gemarkeerde vakje is het midden van je scherm.", 32));
                PixelEditor editor = L.Add(new PixelEditor(s, delegate { Changed(); }));
                L.Add(new SliderRow("\uE740", "Pixelgrootte", 1, 6, s.PixelSize, v => v + "x", v => { s.PixelSize = v; Changed(); }));
                L.Add(new TwoButtons("Tekening gebruiken", delegate
                {
                    s.Style = Settings.StylePixels;
                    Changed();
                }, "Alles wissen", delegate
                {
                    s.Pixels = new string('0', Settings.Grid * Settings.Grid);
                    s.Style = Settings.StylePixels;
                    Changed();
                    editor.Invalidate();
                }));

                R = new Card("\uE91B", "Eigen plaatje", cw);
                R.Add(new InfoRow("Kies een PNG met een doorzichtige achtergrond. Kleine plaatjes (bijv. 32x32) werken het best.", 32));
                R.Add(new ButtonRow("\uE8E5", string.IsNullOrEmpty(s.ImageFile) ? "PNG kiezen..." : "Ander plaatje kiezen (nu: " + s.ImageFile + ")", false, delegate
                {
                    using (OpenFileDialog ofd = new OpenFileDialog())
                    {
                        ofd.Filter = "PNG-plaatjes|*.png";
                        ofd.Title = "Kies je crosshair-plaatje";
                        if (ofd.ShowDialog(this) == DialogResult.OK)
                        {
                            string name = Crosshair.ImportImage(ofd.FileName);
                            if (name == null) { MessageBox.Show(this, "Dit plaatje kon niet worden geladen.", "Custom Crosshair"); return; }
                            s.ImageFile = name;
                            s.Style = Settings.StyleImage;
                            Changed();
                            Rebuild();
                        }
                    }
                }));
                R.Add(new SliderRow("\uE740", "Grootte plaatje", 10, 400, s.ImageScale, v => v + "%", v => { s.ImageScale = v; Changed(); }));
                if (!string.IsNullOrEmpty(s.ImageFile))
                    R.Add(new PrimaryButton(s.Style == Settings.StyleImage ? "Plaatje is actief" : "Plaatje gebruiken", delegate
                    {
                        s.Style = Settings.StyleImage;
                        Changed();
                        Rebuild();
                    }));
                R.Add(new PreviewBox(s));
            }
            else if (page == 1)
            {
                L = new Card("\uE790", "Kleur", cw);
                L.Add(new SwatchRow(Presets, () => s.Color, c => { s.Color = c; Changed(); }));
                L.Add(new ColorRow("\uE790", "Eigen kleur kiezen", () => Crosshair.ColorFor(s), delegate
                {
                    using (ColorDialog cd = new ColorDialog())
                    {
                        cd.Color = s.Color; cd.FullOpen = true;
                        if (cd.ShowDialog(this) == DialogResult.OK) { s.Color = cd.Color; Changed(); }
                    }
                }));
                L.Add(new ToggleRow("\uE706", "RGB-modus", s.Rgb, v => { s.Rgb = v; Changed(); }));
                L.Add(new SliderRow("\uE916", "RGB-snelheid", 1, 10, s.RgbSpeed, v => v.ToString(), v => { s.RgbSpeed = v; Changed(); }));
                L.Add(new ToggleRow("\uE739", "Zwarte rand", s.Outline, v => { s.Outline = v; Changed(); }));

                R = new Card("\uE7B3", "Voorbeeld", cw);
                R.Add(new PreviewBox(s));
                R.Add(new SliderRow("\uE7B3", "Zichtbaarheid", 10, 100, s.Opacity, v => v + "%", v => { s.Opacity = v; Changed(); }));
            }
            else if (page == 2)
            {
                stacked = true;
                int fw = content.Width - SystemInformation.VerticalScrollBarWidth - 4;
                L = new Card("\uE8FD", "Mijn profielen", fw);
                List<string> names = Profiles.Names();
                foreach (string n in names)
                {
                    string name = n;
                    L.Add(new ProfileRow(name, name == s.ActiveProfile, names.Count > 1,
                        () => !Profiles.Matches(s.ActiveProfile, s),
                        delegate { Program.SwitchProfile(name); Rebuild(); },
                        delegate
                        {
                            if (name != s.ActiveProfile &&
                                MessageBox.Show(this, "Je huidige crosshair opslaan in '" + name + "'?\nWat daar nu in staat wordt overschreven.", "Custom Crosshair", MessageBoxButtons.YesNo) != DialogResult.Yes)
                                return;
                            Profiles.Save(name, s);
                            s.ActiveProfile = name;
                            s.Save();
                            Rebuild();
                        },
                        delegate
                        {
                            if (MessageBox.Show(this, "Profiel '" + name + "' verwijderen?", "Custom Crosshair", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            { Profiles.Delete(name); Rebuild(); }
                        }));
                }

                int hw = fw;
                R = new Card("\uE710", "Nieuw profiel", hw);
                R.Add(new InfoRow("Een nieuw profiel is een kopie van je huidige crosshair. Aanpassingen bewaar je met Opslaan.", 34));
                InputRow input = null;
                input = R.Add(new InputRow("Naam, bijv. Roblox", delegate { CreateProfile(input.Value); }));
                R.Add(new ButtonRow("\uE710", "Profiel aanmaken", false, delegate { CreateProfile(input.Value); }));

                C = new Card("\uE8C8", "Crosshair-code", hw);
                C.Add(new CodeRow(() => ShareCode.Make(s)));
                C.Add(new ButtonRow("\uE8C8", "Mijn code kopieren", false, delegate { CopyCode(); }));
                InputRow codeInput = null;
                codeInput = C.Add(new InputRow("Plak hier een code (BC-...)", delegate { UseCode(codeInput.Value); }));
                C.Add(new ButtonRow("\uE896", "Code gebruiken", false, delegate { UseCode(codeInput.Value); }));
            }
            else if (page == 5)
            {
                L = new Card("\uE790", "Accentkleur", cw);
                L.Add(new InfoRow("De kleur van schakelaars, schuifjes, knoppen en het streepje bij elk kopje.", 32));
                L.Add(new SwatchRow(AccentPresets, () => s.Accent, c => { s.Accent = c; s.Save(); BeginInvoke(new Action(ApplyTheme)); }));
                L.Add(new ColorRow("\uE790", "Eigen accentkleur", () => s.Accent, delegate
                {
                    using (ColorDialog cd = new ColorDialog())
                    {
                        cd.Color = s.Accent; cd.FullOpen = true;
                        if (cd.ShowDialog(this) == DialogResult.OK) { s.Accent = cd.Color; s.Save(); BeginInvoke(new Action(ApplyTheme)); }
                    }
                }));

                R = new Card("\uE7B3", "Menu", cw);
                R.Add(new SliderRow("\uE7B3", "Menu zichtbaarheid", 70, 100, s.MenuOpacity, v => v + "%", v =>
                {
                    s.MenuOpacity = v; s.Save();
                    Opacity = v / 100.0;
                }));
            }
            else if (page == 4)
            {
                L = new Card("\uE895", "Updates", cw);
                L.Add(new InfoRow("Klik hieronder om te kijken of er een update is.", 22));
                L.Add(new PrimaryButton("Nu op updates controleren", delegate { Updater.Check(this, true); }));
                L.Add(new ButtonRow("\uE946", "Wat is er nieuw in deze versie", false, delegate { BeginInvoke(new Action(ShowWhatsNew)); }));

                R = new Card("\uE946", "Hoe werkt het", cw);
                R.Add(new InfoRow("Updates zijn verplicht. Bij het opstarten en elke 3 minuten kijkt de app of er een update is. Is die er, dan moet je eerst updaten voordat je verder kunt. De app werkt zichzelf bij en start opnieuw op. Je instellingen en profielen blijven bewaard. Zonder internet kun je de app gewoon gebruiken.", 110));
            }
            else
            {
                L = new Card("\uE713", "Algemeen", cw);
                Screen[] screens = Screen.AllScreens;
                string[] screenNames = new string[screens.Length];
                int screenIdx = 0;
                for (int i = 0; i < screens.Length; i++)
                {
                    screenNames[i] = "Scherm " + (i + 1) + (screens[i].Primary ? " (hoofd)" : "");
                    if (screens[i].DeviceName == Program.TargetScreen(s).DeviceName) screenIdx = i;
                }
                L.Add(new ChoiceRow("\uE7F4", "Crosshair op", screenNames, screenIdx, v =>
                {
                    s.MonitorName = screens[v].DeviceName;
                    Changed();
                }));
                L.Add(new AppRow("\uE7FC", "Alleen tonen in", () => s.TargetApp, v => { s.TargetApp = v; Program.ResetForeground(); Changed(); }));
                L.Add(new InfoRow("Kies je game: de crosshair verdwijnt als je wegtabt. Start de game eerst, dan staat hij in de lijst.", 32));
                L.Add(new KeyRow("\uE765", "Menu-toets", s.MenuKey, k => { s.MenuKey = k; Changed(); }));
                L.Add(new ToggleRow("\uE80F", "Start met Windows", Program.StartupEnabled(), v => Program.SetStartup(v)));
                L.Add(new ToggleRow("\uE7F4", "Menu tonen bij opstarten", s.ShowMenuOnStart, v => { s.ShowMenuOnStart = v; Changed(); }));
                L.Add(new ToggleRow("\uE72E", "Streamproof", s.StreamProof, v =>
                {
                    s.StreamProof = v;
                    Changed();
                    if (!Program.ApplyStreamProof() && v)
                        MessageBox.Show(this, "Streamproof werkt pas vanaf Windows 10 versie 2004.", "Custom Crosshair");
                }));
                L.Add(new InfoRow("Streamproof aan = crosshair en menu zijn onzichtbaar in OBS, Discord-streams, opnames en screenshots. Jij ziet ze gewoon.", 44));

                R = new Card("\uE946", "Overig", cw);
                R.Add(new InfoRow("Alles wordt automatisch opgeslagen. Zet je game op 'Venster zonder rand', anders zie je de crosshair niet.", 48));
                R.Add(new InfoRow("Menukleur", 18));
                R.Add(new SwatchRow(AccentPresets, () => s.Accent, c => { s.Accent = c; s.Save(); BeginInvoke(new Action(ApplyTheme)); }));
                R.Add(new SliderRow("\uE7B3", "Menu zichtbaarheid", 70, 100, s.MenuOpacity, v => v + "%", v => { s.MenuOpacity = v; s.Save(); Opacity = v / 100.0; }));
                R.Add(new ButtonRow("\uE7BE", "Rondleiding opnieuw bekijken", false, delegate { BeginInvoke(new Action(StartTour)); }));
                R.Add(new ButtonRow("\uF3B1", "Uitloggen bij Discord", true, delegate
                {
                    if (MessageBox.Show(this, "Uitloggen bij Discord? De app sluit dan af. Bij het opnieuw openen log je weer in.", "Custom Crosshair", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    { DiscordGate.Logout(s); Program.Quit(); }
                }));
                R.Add(new ButtonRow("\uE77B", "Naam wijzigen", false, delegate
                {
                    using (NameForm nf = new NameForm(s.UserName))
                        if (nf.ShowDialog(this) == DialogResult.OK) { s.UserName = nf.Result; Changed(); }
                }));
                R.Add(new ButtonRow("\uE72C", "Standaard herstellen", false, delegate { s.ResetLook(); Changed(); Rebuild(); }));
                R.Add(new ButtonRow("\uE7E8", "Afsluiten", true, delegate { Program.Quit(); }));
            }


            return new Card[] { L, R, C };
        }

        Panel leftPanel;

        void BuildLeft()
        {
            if (leftPanel == null) return;
            leftPanel.SuspendLayout();
            while (leftPanel.Controls.Count > 0) leftPanel.Controls[0].Dispose();
            leftPanel.BackColor = Theme.Window;
            Card pv = new Card("\uE7B3", "Live voorbeeld", leftPanel.Width);
            pv.Add(new PreviewBox(s, Math.Max(200, leftPanel.Height - 48 - 3 * 34 - 28), 4));
            pv.Add(new ToggleRow("\uE7E8", "Crosshair aan", s.Enabled, v => { s.Enabled = v; Changed(); }));
            pv.Add(new ToggleRow("\uE739", "Zwarte rand", s.Outline, v => { s.Outline = v; Changed(); }));
            pv.Add(new ToggleRow("\uE706", "RGB-modus", s.Rgb, v => { s.Rgb = v; Changed(); }));
            pv.Location = new Point(0, 0);
            leftPanel.Controls.Add(pv);
            leftPanel.ResumeLayout();
        }



        TourDim tourDim; TourBubble tourBubble; int tourStep = -1;

        public void StartTour()
        {
            if (!Visible) Toggle();
            tourStep = 0;
            ShowTourStep();
        }

        void ShowTourStep()
        {
            if (tourBubble != null) { tourBubble.Close(); tourBubble.Dispose(); tourBubble = null; }
            if (tourStep < 0 || tourStep > 3) { EndTour(); return; }
            int H = ClientSize.Height;
            if (tourDim == null) { tourDim = new TourDim(); tourDim.Bounds = Bounds; tourDim.Show(this); }
            tourDim.Bounds = Bounds;
            Rectangle hole = Rectangle.Empty;
            string title = "", text = "";
            if (tourStep == 0)
            {
                hole = new Rectangle(0, 0, 201, H);
                title = "Pagina's";
                text = "Hier kies je wat je wilt aanpassen: je crosshair, kleur, positie, je eigen tekening, profielen en instellingen.";
            }
            else if (tourStep == 1)
            {
                hole = new Rectangle(212, 8, 452, H - 16);
                title = "Aanpassen";
                text = "Hier stel je alles in. Alles wat je verandert wordt meteen opgeslagen.";
            }
            else if (tourStep == 2)
            {
                hole = new Rectangle(664, 50, 326, H - 60);
                title = "Live voorbeeld";
                text = "Hier zie je je crosshair zoals hij in je game komt. Met de schakelaars zet je hem snel aan of uit.";
            }
            else
            {
                title = "Klaar!";
                text = "Druk op " + Theme.KeyName(s.MenuKey) + " om dit menu te openen en te sluiten, ook midden in je game. Veel plezier!";
            }
            tourDim.Hole = hole;
            tourDim.Invalidate();
            tourBubble = new TourBubble(title, text, (tourStep + 1) + " / 4", tourStep == 3,
                delegate { tourStep++; BeginInvoke(new Action(ShowTourStep)); },
                delegate { BeginInvoke(new Action(EndTour)); });
            Point pos;
            if (hole.IsEmpty)
                pos = new Point(Left + (Width - 320) / 2, Top + (Height - 178) / 2);
            else
            {
                Rectangle sh = RectangleToScreen(hole);
                int x = sh.Right + 16;
                if (x + 320 > Right) x = sh.Left - 336;
                if (x < Left) x = sh.Left + (sh.Width - 320) / 2;
                pos = new Point(x, Top + 120);
            }
            tourBubble.Location = pos;
            tourBubble.Show(tourDim);
        }

        void EndTour()
        {
            tourStep = -1;
            if (tourBubble != null) { tourBubble.Close(); tourBubble.Dispose(); tourBubble = null; }
            if (tourDim != null) { tourDim.Close(); tourDim.Dispose(); tourDim = null; }
            if (!s.TourDone) { s.TourDone = true; s.Save(); }
        }

        public void ShowWhatsNew()
        {
            using (WhatsNewForm w = new WhatsNewForm()) w.ShowDialog(this);
        }

        public void Toggle()
        {
            if (Visible)
            {
                if (tourStep >= 0) EndTour();
                Program.CancelCapture();
                Hide();
            }
            else
            {
                BuildContent();
                Show();
                BringToFront();
                Activate();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !Program.Quitting)
            {
                e.Cancel = true;
                Toggle();
                return;
            }
            base.OnFormClosing(e);
        }
    }

    // ------------------------------------------------------------------ toetsenbord
    public static class KeyHook
    {
        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")] static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint threadId);
        [DllImport("user32.dll")] static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")] static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)] static extern IntPtr GetModuleHandle(string name);

        static HookProc proc = Callback;
        static IntPtr hook = IntPtr.Zero;
        static int lastDown = -1;
        public static Action<int> OnKeyDown;

        [StructLayout(LayoutKind.Sequential)]
        struct MSG { public IntPtr hwnd; public uint message; public IntPtr wParam; public IntPtr lParam; public uint time; public int ptX; public int ptY; }
        [DllImport("user32.dll")] static extern int GetMessage(out MSG m, IntPtr h, uint min, uint max);
        [DllImport("user32.dll")] static extern bool PostThreadMessage(uint id, uint msg, IntPtr w, IntPtr l);
        [DllImport("kernel32.dll")] static extern uint GetCurrentThreadId();
        static uint hookThread = 0;

        // De hook draait op een eigen draad, zodat toetsaanslagen nooit wachten op het menu.
        public static void Start()
        {
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {
                hookThread = GetCurrentThreadId();
                hook = SetWindowsHookEx(13, proc, GetModuleHandle(null), 0);
                MSG m;
                while (GetMessage(out m, IntPtr.Zero, 0, 0) > 0) { }
                if (hook != IntPtr.Zero) UnhookWindowsHookEx(hook);
                hook = IntPtr.Zero;
            });
            th.IsBackground = true;
            th.Priority = System.Threading.ThreadPriority.AboveNormal;
            th.Start();
        }

        public static void Stop()
        {
            if (hookThread != 0) PostThreadMessage(hookThread, 0x0012, IntPtr.Zero, IntPtr.Zero);
            hookThread = 0;
        }

        static IntPtr Callback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();
                int vk = Marshal.ReadInt32(lParam);
                if (msg == 0x100 || msg == 0x104)
                {
                    if (vk != lastDown)
                    {
                        lastDown = vk;
                        if (OnKeyDown != null) OnKeyDown(vk);
                    }
                }
                else if (msg == 0x101 || msg == 0x105)
                {
                    if (vk == lastDown) lastDown = -1;
                }
            }
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }
    }

    // ------------------------------------------------------------------ FiveM (alleen lokaal opgeslagen)
    public class FServer
    {
        public string Code = "", Name = "", Profile = "";
        public bool Loaded, Online;
        public int Clients, Max;
    }

    public static class FiveM
    {
        public static List<FServer> Servers = new List<FServer>();
        static bool busy = false;
        static string FilePath { get { return Path.Combine(Settings.Dir, "servers.txt"); } }

        public static void Load()
        {
            Servers.Clear();
            try
            {
                if (!File.Exists(FilePath)) return;
                foreach (string line in File.ReadAllLines(FilePath))
                {
                    string[] p = line.Split('|');
                    if (p.Length < 1 || p[0].Trim().Length == 0) continue;
                    FServer f = new FServer();
                    f.Code = p[0].Trim();
                    f.Name = p.Length > 1 ? p[1] : f.Code;
                    f.Profile = p.Length > 2 ? p[2] : "";
                    Servers.Add(f);
                }
            }
            catch { }
        }

        public static void Save()
        {
            try
            {
                Directory.CreateDirectory(Settings.Dir);
                List<string> l = new List<string>();
                foreach (FServer f in Servers) l.Add(f.Code + "|" + f.Name.Replace("|", "") + "|" + f.Profile.Replace("|", ""));
                File.WriteAllLines(FilePath, l.ToArray());
            }
            catch { }
        }

        public static bool IsDirect(string code) { return code.Contains(":"); }

        public static string ParseCode(string input)
        {
            if (input == null) return null;
            input = input.Trim();
            if (input.StartsWith("connect ", StringComparison.OrdinalIgnoreCase)) input = input.Substring(8).Trim();
            input = System.Text.RegularExpressions.Regex.Replace(input, "^fivem://connect/", "", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, "join/([A-Za-z0-9]+)");
            if (m.Success) return m.Groups[1].Value.ToLower();
            System.Text.RegularExpressions.Match d = System.Text.RegularExpressions.Regex.Match(input,
                "^((?:\\d{1,3}\\.){3}\\d{1,3}|[A-Za-z0-9-]+(?:\\.[A-Za-z0-9-]+)+)(?::(\\d{2,5}))?$");
            if (d.Success) return d.Groups[1].Value.ToLower() + ":" + (d.Groups[2].Success ? d.Groups[2].Value : "30120");
            if (System.Text.RegularExpressions.Regex.IsMatch(input, "^[A-Za-z0-9]{4,10}$")) return input.ToLower();
            return null;
        }

        static string Clean(string name)
        {
            try { name = System.Text.RegularExpressions.Regex.Unescape(name); } catch { }
            name = System.Text.RegularExpressions.Regex.Replace(name, "\\^[0-9]", "");
            name = System.Text.RegularExpressions.Regex.Replace(name, "\\s+", " ").Trim();
            if (name.Length > 40) name = name.Substring(0, 40).Trim() + "...";
            return name;
        }

        static string GetText(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.UserAgent = "Mozilla/5.0 CustomCrosshair";
            req.Timeout = 6000; req.ReadWriteTimeout = 6000;
            using (System.Net.WebResponse r = req.GetResponse())
            using (StreamReader sr = new StreamReader(r.GetResponseStream())) return sr.ReadToEnd();
        }

        static void FetchDirect(FServer f)
        {
            try
            {
                string json = GetText("http://" + f.Code + "/dynamic.json");
                System.Text.RegularExpressions.Match c = System.Text.RegularExpressions.Regex.Match(json, "\"clients\"\\s*:\\s*(\\d+)");
                System.Text.RegularExpressions.Match mx = System.Text.RegularExpressions.Regex.Match(json, "\"sv_maxclients\"\\s*:\\s*\"?(\\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                System.Text.RegularExpressions.Match hn = System.Text.RegularExpressions.Regex.Match(json, "\"hostname\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                f.Clients = c.Success ? int.Parse(c.Groups[1].Value) : 0;
                f.Max = mx.Success ? int.Parse(mx.Groups[1].Value) : 0;
                f.Online = true;
                string name = hn.Success ? hn.Groups[1].Value : "";
                try
                {
                    string info = GetText("http://" + f.Code + "/info.json");
                    System.Text.RegularExpressions.Match pn = System.Text.RegularExpressions.Regex.Match(info, "\"sv_projectName\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                    if (pn.Success && pn.Groups[1].Value.Trim().Length > 0) name = pn.Groups[1].Value;
                }
                catch { }
                if (name.Length > 0) f.Name = Clean(name);
            }
            catch { f.Online = false; f.Clients = 0; }
            f.Loaded = true;
        }

        static void Fetch(FServer f)
        {
            if (IsDirect(f.Code)) { FetchDirect(f); return; }
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://servers-frontend.fivem.net/api/servers/single/" + f.Code);
                req.UserAgent = "Mozilla/5.0 CustomCrosshair";
                req.Timeout = 7000; req.ReadWriteTimeout = 7000;
                string json;
                using (System.Net.WebResponse r = req.GetResponse())
                using (StreamReader sr = new StreamReader(r.GetResponseStream())) json = sr.ReadToEnd();
                System.Text.RegularExpressions.Match c = System.Text.RegularExpressions.Regex.Match(json, "\"clients\"\\s*:\\s*(\\d+)");
                System.Text.RegularExpressions.Match mx = System.Text.RegularExpressions.Regex.Match(json, "\"(?:sv_maxclients|svMaxclients)\"\\s*:\\s*\"?(\\d+)");
                System.Text.RegularExpressions.Match pn = System.Text.RegularExpressions.Regex.Match(json, "\"sv_projectName\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                System.Text.RegularExpressions.Match hn = System.Text.RegularExpressions.Regex.Match(json, "\"hostname\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                f.Clients = c.Success ? int.Parse(c.Groups[1].Value) : 0;
                f.Max = mx.Success ? int.Parse(mx.Groups[1].Value) : 0;
                string name = pn.Success && pn.Groups[1].Value.Trim().Length > 0 ? pn.Groups[1].Value : (hn.Success ? hn.Groups[1].Value : "");
                if (name.Length > 0) f.Name = Clean(name);
                f.Online = c.Success;
            }
            catch { f.Online = false; f.Clients = 0; }
            f.Loaded = true;
        }

        public static void Refresh(Control ui, Action done)
        {
            if (busy) return;
            busy = true;
            List<FServer> copy = new List<FServer>(Servers);
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {
                foreach (FServer f in copy) Fetch(f);
                try { ui.BeginInvoke(new Action(delegate { busy = false; Save(); done(); })); } catch { busy = false; }
            });
            th.IsBackground = true;
            th.Start();
        }

        public static void Join(FServer f)
        {
            try { System.Diagnostics.Process.Start("fivem://connect/" + (IsDirect(f.Code) ? f.Code : "cfx.re/join/" + f.Code)); }
            catch { MessageBox.Show("FiveM kon niet worden gestart. Is FiveM geinstalleerd?", "Custom Crosshair"); }
        }

        static bool FiveMRunning()
        {
            bool running = false;
            foreach (System.Diagnostics.Process pr in System.Diagnostics.Process.GetProcesses())
                using (pr)
                {
                    try { if (pr.ProcessName.StartsWith("FiveM", StringComparison.OrdinalIgnoreCase)) running = true; } catch { }
                }
            return running;
        }

        public static void ClearCache(IWin32Window owner)
        {
            string data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FiveM\\FiveM.app\\data");
            if (!Directory.Exists(data)) { MessageBox.Show(owner, "FiveM is niet gevonden op de standaardplek.", "Custom Crosshair"); return; }
            if (FiveMRunning()) { MessageBox.Show(owner, "Sluit FiveM eerst helemaal af en probeer het dan opnieuw.", "Custom Crosshair"); return; }
            string[] dirs = new string[] { "cache", "server-cache", "server-cache-priv" };
            long size = 0;
            foreach (string d in dirs)
            {
                string pth = Path.Combine(data, d);
                try { if (Directory.Exists(pth)) foreach (string f in Directory.GetFiles(pth, "*", SearchOption.AllDirectories)) size += new FileInfo(f).Length; } catch { }
            }
            if (MessageBox.Show(owner, "FiveM-cache legen? (" + (size / (1024 * 1024)) + " MB)\n\nFiveM laadt alles daarna opnieuw, de eerste keer joinen duurt dus wat langer.",
                    "Custom Crosshair", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            int failed = 0;
            foreach (string d in dirs)
            {
                string pth = Path.Combine(data, d);
                try { if (Directory.Exists(pth)) Directory.Delete(pth, true); } catch { failed++; }
            }
            MessageBox.Show(owner, failed == 0 ? "De FiveM-cache is geleegd." : "Een deel kon niet worden verwijderd. Is FiveM echt dicht?", "Custom Crosshair");
        }
    }

    // ------------------------------------------------------------------ Discord-login
    public static class DiscordGate
    {
        public const string ClientId = "1569303356315402351";
        public const string GuildId = "1569304769242075238";
        public const string RoleId = "1574492909690949712";
        public const string Invite = "https://discord.gg/DaVBmyWxkD";
        public const string TicketUrl = "https://discord.com/channels/1569304769242075238/1569313268680368192";
        const int Port = 53682;
        public enum Result { Ok, NotMember, NoRole, NeedLogin, Error }
        public static string DiscordName = null;
        public static string LastError = "";

        static long Now { get { return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds; } }

        static string Token(Settings s)
        {
            if (string.IsNullOrEmpty(s.DiscordToken) || s.DiscordExpires < Now + 60) return null;
            try
            {
                byte[] enc = Convert.FromBase64String(s.DiscordToken);
                byte[] dec = System.Security.Cryptography.ProtectedData.Unprotect(enc, null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
                return System.Text.Encoding.UTF8.GetString(dec);
            }
            catch { return null; }
        }

        static void SaveToken(Settings s, string token, int expiresIn)
        {
            byte[] enc = System.Security.Cryptography.ProtectedData.Protect(System.Text.Encoding.UTF8.GetBytes(token), null, System.Security.Cryptography.DataProtectionScope.CurrentUser);
            s.DiscordToken = Convert.ToBase64String(enc);
            s.DiscordExpires = Now + Math.Max(600, expiresIn);
            s.Save();
        }

        public static void Logout(Settings s)
        {
            s.DiscordToken = ""; s.DiscordExpires = 0; s.DiscordVerifiedUntil = 0;
            s.Save();
        }

        public static bool RecentlyVerified(Settings s) { return s.DiscordVerifiedUntil > Now; }

        public static Result Check(Settings s)
        {
            string tok = Token(s);
            if (tok == null) return Result.NeedLogin;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://discord.com/api/v10/users/@me/guilds/" + GuildId + "/member");
                req.UserAgent = "CustomCrosshair (https://github.com/Basssie91/CustomCrosshair, 1.0)";
                req.Headers.Add("Authorization", "Bearer " + tok);
                req.Timeout = 7000; req.ReadWriteTimeout = 7000;
                string json;
                using (System.Net.WebResponse r = req.GetResponse())
                using (StreamReader sr = new StreamReader(r.GetResponseStream())) json = sr.ReadToEnd();
                System.Text.RegularExpressions.Match gn = System.Text.RegularExpressions.Regex.Match(json, "\"global_name\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                System.Text.RegularExpressions.Match un = System.Text.RegularExpressions.Regex.Match(json, "\"username\"\\s*:\\s*\"((?:[^\"\\\\]|\\\\.)*)\"");
                string nm = gn.Success ? gn.Groups[1].Value : (un.Success ? un.Groups[1].Value : null);
                if (nm != null) { try { nm = System.Text.RegularExpressions.Regex.Unescape(nm); } catch { } DiscordName = nm; }
                System.Text.RegularExpressions.Match roles = System.Text.RegularExpressions.Regex.Match(json, "\"roles\"\\s*:\\s*\\[([^\\]]*)\\]");
                bool has = roles.Success && roles.Groups[1].Value.Contains("\"" + RoleId + "\"");
                if (!has) return Result.NoRole;
                s.DiscordVerifiedUntil = Now + 7 * 24 * 3600;
                s.Save();
                return Result.Ok;
            }
            catch (System.Net.WebException we)
            {
                System.Net.HttpWebResponse resp = we.Response as System.Net.HttpWebResponse;
                if (resp != null)
                {
                    int code = (int)resp.StatusCode;
                    if (code == 404) return Result.NotMember;
                    if (code == 401 || code == 403) { s.DiscordToken = ""; s.DiscordExpires = 0; s.Save(); return Result.NeedLogin; }
                    LastError = "Discord gaf foutcode " + code + ".";
                }
                else LastError = "Geen verbinding met Discord.";
                return Result.Error;
            }
            catch (Exception ex) { LastError = ex.Message; return Result.Error; }
        }

        static void Send(System.Net.Sockets.NetworkStream ns, string status, string type, string body)
        {
            byte[] b = System.Text.Encoding.UTF8.GetBytes(body);
            string head = "HTTP/1.1 " + status + "\r\nContent-Type: " + type + "; charset=utf-8\r\nContent-Length: " + b.Length + "\r\nConnection: close\r\n\r\n";
            byte[] h = System.Text.Encoding.ASCII.GetBytes(head);
            ns.Write(h, 0, h.Length);
            ns.Write(b, 0, b.Length);
            ns.Flush();
        }

        const string Page = @"<!doctype html><html><head><meta charset=""utf-8""><title>Custom Crosshair</title>
<style>body{background:#1b1f29;color:#e8eaf0;font-family:'Segoe UI',sans-serif;display:flex;align-items:center;justify-content:center;height:100vh;margin:0}
div{text-align:center}h1{font-size:24px;margin:0 0 8px}p{color:#8c94a5}</style></head>
<body><div><h1 id=""t"">Even geduld...</h1><p id=""p""></p></div><script>
var h=location.hash.substring(1);var q=new URLSearchParams(h);
if(q.get('access_token')){fetch('/token?'+h).then(function(){document.getElementById('t').textContent='Ingelogd!';
document.getElementById('p').textContent='Je kunt dit tabblad sluiten en teruggaan naar Custom Crosshair.';});}
else{document.getElementById('t').textContent='Inloggen geannuleerd';document.getElementById('p').textContent='Ga terug naar Custom Crosshair en probeer het opnieuw.';
fetch('/cancel');}
</script></body></html>";

        // Opent de browser en wacht tot Discord het inlogtoken teruggeeft (max 3 minuten).
        public static bool Login(Settings s, Func<bool> cancelled, Control ui)
        {
            string state = Guid.NewGuid().ToString("N");
            string redirect = "http://127.0.0.1:" + Port + "/callback";
            string url = "https://discord.com/oauth2/authorize?client_id=" + ClientId +
                "&response_type=token&redirect_uri=" + Uri.EscapeDataString(redirect) +
                "&scope=" + Uri.EscapeDataString("identify guilds.members.read") + "&state=" + state;
            System.Net.Sockets.TcpListener l = null;
            try
            {
                l = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, Port);
                l.Start();
            }
            catch { LastError = "Poort " + Port + " is bezet. Sluit andere programma's en probeer opnieuw."; return false; }
            try
            {
                ui.Invoke(new Action(delegate { Open(url); }));
                DateTime end = DateTime.Now.AddMinutes(3);
                while (DateTime.Now < end && !cancelled())
                {
                    if (!l.Pending()) { System.Threading.Thread.Sleep(100); continue; }
                    using (System.Net.Sockets.TcpClient c = l.AcceptTcpClient())
                    {
                        c.ReceiveTimeout = 3000;
                        System.Net.Sockets.NetworkStream ns = c.GetStream();
                        StreamReader sr = new StreamReader(ns);
                        string line = sr.ReadLine() ?? "";
                        string[] parts = line.Split(' ');
                        string path = parts.Length > 1 ? parts[1] : "/";
                        if (path.StartsWith("/token?"))
                        {
                            Dictionary<string, string> q = new Dictionary<string, string>();
                            foreach (string kv in path.Substring(7).Split('&'))
                            {
                                int i = kv.IndexOf('=');
                                if (i > 0) q[kv.Substring(0, i)] = Uri.UnescapeDataString(kv.Substring(i + 1));
                            }
                            Send(ns, "200 OK", "text/plain", "ok");
                            string tok, st, ex;
                            if (q.TryGetValue("access_token", out tok) && q.TryGetValue("state", out st) && st == state)
                            {
                                int exp = 604800;
                                if (q.TryGetValue("expires_in", out ex)) int.TryParse(ex, out exp);
                                SaveToken(s, tok, exp);
                                return true;
                            }
                        }
                        else if (path.StartsWith("/cancel"))
                        {
                            Send(ns, "200 OK", "text/plain", "ok");
                            LastError = "Inloggen is geannuleerd.";
                            return false;
                        }
                        else if (path.StartsWith("/callback")) Send(ns, "200 OK", "text/html", Page);
                        else Send(ns, "404 Not Found", "text/plain", "");
                    }
                }
                LastError = "Inloggen duurde te lang. Probeer het opnieuw.";
                return false;
            }
            catch (Exception e) { LastError = e.Message; return false; }
            finally { try { l.Stop(); } catch { } }
        }

        public static void Open(string url)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(url);
                psi.UseShellExecute = true;
                System.Diagnostics.Process.Start(psi);
            }
            catch
            {
                try { System.Diagnostics.Process.Start("explorer.exe", "\"" + url + "\""); } catch { }
            }
        }
    }

    public class AccessForm : Form
    {
        Settings s; DiscordGate.Result state; bool busy, closing;
        string title = "", text = "";
        PrimaryButton main; LinkRow second, quit;
        Bitmap logo = AppIcon.MakeBitmap(112);
        [DllImport("dwmapi.dll")] static extern int DwmSetWindowAttribute(IntPtr h, int attr, ref int val, int size);
        [DllImport("user32.dll")] static extern bool ReleaseCapture();
        [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr h, int msg, IntPtr w, IntPtr l);

        public AccessForm(Settings s, DiscordGate.Result start)
        {
            this.s = s;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(440, 300);
            BackColor = Theme.Window;
            TopMost = true;
            ShowInTaskbar = true;
            Text = "Custom Crosshair";
            Icon = AppIcon.MakeIcon(32);
            DoubleBuffered = true;
            main = new PrimaryButton("", delegate { MainClick(); });
            main.BackColor = Theme.Window;
            main.SetBounds(30, 196, 380, 40);
            Controls.Add(main);
            second = new LinkRow("", delegate { Recheck(); });
            second.BackColor = Theme.Window;
            second.SetBounds(30, 248, 200, 26);
            Controls.Add(second);
            quit = new LinkRow("Afsluiten", delegate { closing = true; DialogResult = DialogResult.Cancel; Close(); });
            quit.BackColor = Theme.Window;
            quit.SetBounds(330, 248, 80, 26);
            Controls.Add(quit);
            SetState(start);
        }

        void SetState(DiscordGate.Result r)
        {
            state = r; busy = false;
            second.Visible = true;
            switch (r)
            {
                case DiscordGate.Result.Ok:
                    DialogResult = DialogResult.OK; Close(); return;
                case DiscordGate.Result.NotMember:
                    title = "Join eerst de Discord";
                    text = "Custom Crosshair is alleen voor leden van onze Discord-server. Join de server en klik daarna op Opnieuw controleren.";
                    SetMain("Server joinen"); SetSecond("Opnieuw controleren");
                    break;
                case DiscordGate.Result.NoRole:
                    title = "Je hebt nog geen toegang";
                    text = "Je zit in de server, maar je hebt de rol voor Custom Crosshair nog niet. Maak een ticket aan om toegang aan te vragen.";
                    SetMain("Ticket aanmaken"); SetSecond("Opnieuw controleren");
                    break;
                case DiscordGate.Result.Error:
                    title = "Er ging iets mis";
                    text = DiscordGate.LastError + " Controleer je internet en probeer het opnieuw.";
                    SetMain("Opnieuw proberen"); SetSecond("Opnieuw inloggen");
                    break;
                default:
                    title = "Inloggen met Discord";
                    text = "Custom Crosshair is alleen voor leden van onze Discord-server. Log in met Discord om verder te gaan.";
                    if (DiscordGate.LastError.Length > 0) text += "\n" + DiscordGate.LastError;
                    SetMain("Inloggen met Discord"); second.Visible = false;
                    break;
            }
            Invalidate();
        }

        void SetMain(string t2) { Controls.Remove(main); main.Dispose(); main = new PrimaryButton(t2, delegate { MainClick(); }); main.BackColor = Theme.Window; main.SetBounds(30, 196, 380, 40); Controls.Add(main); }
        void SetSecond(string t2) { Controls.Remove(second); second.Dispose(); second = new LinkRow(t2, delegate { SecondClick(); }); second.BackColor = Theme.Window; second.SetBounds(30, 248, 240, 26); Controls.Add(second); }

        void Busy(string msg) { busy = true; title = msg; text = "Even geduld..."; Invalidate(); }

        void MainClick()
        {
            if (busy) return;
            if (state == DiscordGate.Result.NotMember) { DiscordGate.Open(DiscordGate.Invite); return; }
            if (state == DiscordGate.Result.NoRole) { DiscordGate.Open(DiscordGate.TicketUrl); return; }
            if (state == DiscordGate.Result.Error) { Recheck(); return; }
            DoLogin();
        }

        void SecondClick()
        {
            if (busy) return;
            if (state == DiscordGate.Result.Error) { DoLogin(); return; }
            Recheck();
        }

        void DoLogin()
        {
            Busy("Kijk in je browser...");
            text = "Log daar in met Discord en klik op Autoriseren. Daarna gaat het hier vanzelf verder.";
            Invalidate();
            DiscordGate.LastError = "";
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {
                bool ok = DiscordGate.Login(s, () => closing, this);
                DiscordGate.Result r = ok ? DiscordGate.Check(s) : DiscordGate.Result.NeedLogin;
                try { BeginInvoke(new Action(delegate { Activate(); SetState(r); })); } catch { }
            });
            th.IsBackground = true;
            th.Start();
        }

        void Recheck()
        {
            if (busy) return;
            Busy("Controleren...");
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {
                DiscordGate.Result r = DiscordGate.Check(s);
                try { BeginInvoke(new Action(delegate { SetState(r); })); } catch { }
            });
            th.IsBackground = true;
            th.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e) { closing = true; base.OnFormClosing(e); }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            try { int v = 2; DwmSetWindowAttribute(Handle, 33, ref v, 4); } catch { }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left) { ReleaseCapture(); SendMessage(Handle, 0xA1, (IntPtr)2, IntPtr.Zero); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using (Pen p = new Pen(Theme.Border)) g.DrawRectangle(p, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(logo, 30, 28, 40, 40);
            Theme.Label(g, "Custom Crosshair", Theme.Bold, Theme.Dim, new Rectangle(80, 28, 300, 20), TextFormatFlags.Left);
            Theme.Label(g, title, Theme.Title, Theme.Text, new Rectangle(80, 46, 340, 24), TextFormatFlags.Left);
            TextRenderer.DrawText(g, text, Theme.Normal, new Rectangle(30, 96, 380, 90), Theme.Dim, TextFormatFlags.WordBreak | TextFormatFlags.Left);
        }
    }

    // ------------------------------------------------------------------ updates via GitHub
    public static class Updater
    {
        public const string Version = "1.8.2";
        public static readonly string[] Notes = new string[] {
            "Updates worden nu direct en automatisch geinstalleerd, ook als de app open staat",
            "Inloggen met Discord: de app is alleen voor leden met toegang",
            "Je naam wordt automatisch overgenomen van Discord",
            "Uitloggen bij Discord via Instellingen" };
        const string Repo = "Basssie91/CustomCrosshair";
        static bool busy = false;
        static bool forcing = false;

        static string Get(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.UserAgent = "CustomCrosshair";
            req.Timeout = 6000;
            req.ReadWriteTimeout = 6000;
            using (System.Net.WebResponse r = req.GetResponse())
            using (StreamReader sr = new StreamReader(r.GetResponseStream()))
                return sr.ReadToEnd();
        }

        static string etag = null, cachedJson = null;

        // Vraagt GitHub alleen om nieuwe gegevens als er echt iets veranderd is (telt niet mee voor de limiet)
        static string GetCached(string url)
        {
            System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
            System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
            req.UserAgent = "CustomCrosshair";
            req.Timeout = 6000;
            req.ReadWriteTimeout = 6000;
            if (etag != null && cachedJson != null) req.Headers.Add("If-None-Match", etag);
            try
            {
                using (System.Net.HttpWebResponse r = (System.Net.HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(r.GetResponseStream()))
                {
                    string body = sr.ReadToEnd();
                    string et = r.Headers["ETag"];
                    if (!string.IsNullOrEmpty(et)) { etag = et; cachedJson = body; }
                    return body;
                }
            }
            catch (System.Net.WebException we)
            {
                System.Net.HttpWebResponse resp = we.Response as System.Net.HttpWebResponse;
                if (resp != null && (int)resp.StatusCode == 304 && cachedJson != null) return cachedJson;
                throw;
            }
        }

        // true = er is een nieuwere versie; url = download van de .exe
        public static bool UpdateAvailable(out string url, out string error)
        {
            url = null; error = null;
            try
            {
                string json = GetCached("https://api.github.com/repos/" + Repo + "/releases/latest");
                System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(json, "\"tag_name\"\\s*:\\s*\"v?([0-9.]+)\"");
                System.Text.RegularExpressions.Match u = System.Text.RegularExpressions.Regex.Match(json,
                    "\"browser_download_url\"\\s*:\\s*\"(https://github\\.com/" + Repo + "/releases/download/[^\"]+?\\.exe)\"");
                if (!m.Success || !u.Success) { error = "Geen update gevonden op GitHub."; return false; }
                url = u.Groups[1].Value;
                return IsNewer(m.Groups[1].Value, Version);
            }
            catch (Exception ex) { error = ex.Message; return false; }
        }

        static bool IsNewer(string a, string b)
        {
            try { return new System.Version(a) > new System.Version(b); } catch { return false; }
        }

        // Verplichte update: alleen updaten of afsluiten. Komt nooit terug.
        public static void ForceUpdate(string url)
        {
            if (forcing) return;
            forcing = true;
            Program.HideForUpdate();
            using (UpdateNotice n = new UpdateNotice()) n.ShowDialog();
            Install(url);
            Program.ExitNow();
        }

        static void Install(string url)
        {
            Form wait = new Form();
            wait.FormBorderStyle = FormBorderStyle.None;
            wait.StartPosition = FormStartPosition.CenterScreen;
            wait.Size = new Size(300, 70);
            wait.BackColor = Theme.Window;
            wait.TopMost = true;
            wait.ShowInTaskbar = false;
            Label l = new Label();
            l.Text = "Bezig met updaten...";
            l.ForeColor = Theme.Text;
            l.Font = Theme.HeavySmall;
            l.Dock = DockStyle.Fill;
            l.TextAlign = ContentAlignment.MiddleCenter;
            wait.Controls.Add(l);
            wait.Show();
            wait.Refresh();

            string tmp = Path.Combine(Path.GetTempPath(), "CustomCrosshair_nieuw.exe");
            string bat = Path.Combine(Path.GetTempPath(), "CustomCrosshair_update.bat");
            string current = Application.ExecutablePath;
            try
            {
                System.Net.ServicePointManager.SecurityProtocol |= (System.Net.SecurityProtocolType)3072;
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    wc.Headers.Add("User-Agent", "CustomCrosshair");
                    wc.DownloadFile(url, tmp);
                }
                if (new FileInfo(tmp).Length < 10000) throw new Exception("De download is mislukt.");
                File.WriteAllText(bat,
                    "@echo off\r\n" +
                    "set n=0\r\n" +
                    "timeout /t 2 /nobreak >nul\r\n" +
                    ":retry\r\n" +
                    "set /a n+=1\r\n" +
                    "copy /y \"" + tmp + "\" \"" + current + "\" >nul 2>&1 && goto ok\r\n" +
                    "if %n% gtr 15 goto end\r\n" +
                    "timeout /t 1 /nobreak >nul\r\n" +
                    "goto retry\r\n" +
                    ":ok\r\n" +
                    "start \"\" \"" + current + "\"\r\n" +
                    ":end\r\n" +
                    "del \"" + tmp + "\" >nul 2>&1\r\n" +
                    "del \"%~f0\" >nul 2>&1\r\n", System.Text.Encoding.Default);
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/c \"" + bat + "\"");
                psi.CreateNoWindow = true;
                psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                psi.UseShellExecute = false;
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                wait.Close();
                MessageBox.Show("Updaten is mislukt:\n" + ex.Message + "\n\nDe app wordt afgesloten. Probeer het later opnieuw.", "Custom Crosshair");
            }
        }

        // Controle op de achtergrond (bij de knop en elk half uur)
        public static void Check(Control ui, bool manual)
        {
            if (busy || forcing) return;
            busy = true;
            System.Threading.Thread th = new System.Threading.Thread(delegate()
            {
                string url, error;
                bool available = UpdateAvailable(out url, out error);
                try
                {
                    ui.BeginInvoke(new Action(delegate
                    {
                        busy = false;
                        if (available) { Program.HideForUpdate(); ForceUpdate(url); return; }
                        if (!manual) return;
                        if (error != null) MessageBox.Show("Kon niet controleren op updates.\n" + error, "Custom Crosshair");
                        else MessageBox.Show("Je app is up-to-date.", "Custom Crosshair");
                    }));
                }
                catch { busy = false; }
            });
            th.IsBackground = true;
            th.Start();
        }
    }

    // ------------------------------------------------------------------ programma
    public static class Program
    {
        [DllImport("user32.dll")] static extern bool SetProcessDPIAware();
        [DllImport("user32.dll")] static extern bool SetWindowDisplayAffinity(IntPtr hWnd, uint affinity);

        public static bool ApplyStreamProof()
        {
            uint a = S.StreamProof ? 0x11u : 0u;
            bool ok = true;
            try
            {
                if (!SetWindowDisplayAffinity(overlay.Handle, a)) ok = false;
                if (!SetWindowDisplayAffinity(menu.Handle, a)) ok = false;
            }
            catch { ok = false; }
            return ok;
        }

        public static bool Quitting = false;
        public static string ScriptPath = "";
        static NotifyIcon tray;
        static Settings S;
        static MenuForm menu;
        static Overlay overlay;
        static Action<int> capture;

        static string StartupFile
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "CustomCrosshair.bat"); }
        }

        static void Migrate()
        {
            try
            {
                string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string oldDir = Path.Combine(appData, "BasCrosshair");
                if (Directory.Exists(oldDir) && !Directory.Exists(Settings.Dir)) Directory.Move(oldDir, Settings.Dir);
                string oldStart = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "BasCrosshair.bat");
                if (File.Exists(oldStart)) { File.Delete(oldStart); SetStartup(true); }
            }
            catch { }
        }

        public static bool StartupEnabled() { return File.Exists(StartupFile); }

        public static void SetStartup(bool on)
        {
            try
            {
                if (on)
                    File.WriteAllText(StartupFile, "@echo off\r\nstart \"\" \"" + ScriptPath + "\"\r\n", System.Text.Encoding.Default);
                else if (File.Exists(StartupFile))
                    File.Delete(StartupFile);
            }
            catch (Exception ex) { MessageBox.Show("Dat lukte niet: " + ex.Message, "Custom Crosshair"); }
        }

        public static void BeginCapture(Action<int> cb) { capture = cb; }
        public static void CancelCapture()
        {
            if (capture != null) { Action<int> c = capture; capture = null; c(0x1B); }
        }

        static void HandleKey(int vk)
        {
            if (capture != null)
            {
                Action<int> c = capture;
                capture = null;
                c(vk);
                return;
            }
            if (vk == S.MenuKey) menu.Toggle();
        }

        [DllImport("user32.dll")] static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(IntPtr h, out uint pid);
        static IntPtr lastFg = new IntPtr(-1);
        static uint lastPid = 0;
        static string lastName = "";
        static uint ownPid = (uint)System.Diagnostics.Process.GetCurrentProcess().Id;

        public static void ResetForeground() { lastFg = new IntPtr(-1); }

        static void CheckForeground()
        {
            bool allowed = true;
            if (!string.IsNullOrEmpty(S.TargetApp))
            {
                IntPtr fg = GetForegroundWindow();
                if (fg != lastFg)
                {
                    lastFg = fg;
                    uint pid = 0;
                    GetWindowThreadProcessId(fg, out pid);
                    lastPid = pid;
                    try
                    {
                        if (pid == 0) lastName = "";
                        else using (System.Diagnostics.Process pr = System.Diagnostics.Process.GetProcessById((int)pid)) lastName = pr.ProcessName;
                    }
                    catch { lastName = ""; }
                }
                allowed = lastPid == ownPid || string.Equals(lastName, S.TargetApp, StringComparison.OrdinalIgnoreCase);
            }
            if (allowed != overlay.AppAllowed)
            {
                overlay.AppAllowed = allowed;
                overlay.ApplySettings();
            }
        }

        public static Screen TargetScreen(Settings s)
        {
            if (!string.IsNullOrEmpty(s.MonitorName))
                foreach (Screen sc in Screen.AllScreens)
                    if (sc.DeviceName == s.MonitorName) return sc;
            return Screen.PrimaryScreen;
        }

        public static void SwitchProfile(string name)
        {
            if (!Profiles.Exists(name)) return;
            Profiles.Load(name, S);
            S.ActiveProfile = name;
            S.Save();
            overlay.ApplySettings();
            menu.RefreshAll();
        }

        public static void HideForUpdate()
        {
            try
            {
                if (overlay != null) overlay.Hide();
                if (menu != null) menu.Hide();
            }
            catch { }
        }

        public static void ExitNow()
        {
            Quitting = true;
            try { KeyHook.Stop(); } catch { }
            try { if (tray != null) { tray.Visible = false; tray.Dispose(); } } catch { }
            Environment.Exit(0);
        }

        public static void Quit()
        {
            Quitting = true;
            KeyHook.Stop();
            if (tray != null) { tray.Visible = false; tray.Dispose(); }
            Application.Exit();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Run(Application.ExecutablePath);
        }

        public static void Run(string scriptPath)
        {
            ScriptPath = scriptPath;
            Migrate();
            bool created;
            System.Threading.Mutex mutex = new System.Threading.Mutex(true, "CustomCrosshair_EnkeleInstantie", out created);
            if (!created)
            {
                MessageBox.Show("Custom Crosshair draait al.\nDruk op je menu-toets (standaard HOME) om het menu te openen.", "Custom Crosshair");
                return;
            }

            try { SetProcessDPIAware(); } catch { }
            try { Application.EnableVisualStyles(); } catch { }

            S = Settings.Load();

            if (!S.StyleV2)
            {
                S.MenuTheme = 4;
                S.Accent = Color.FromArgb(34, 211, 238);
                S.StyleV2 = true;
                S.Save();
            }
            Theme.Apply(S.MenuTheme, S.Accent);
            FiveM.Load();

            // eenmalig: iedereen opnieuw laten beginnen (naam + rondleiding)
            if (S.OnboardV < 1)
            {
                S.UserName = "";
                S.TourDone = false;
                S.OnboardV = 1;
                S.Save();
            }

            // opstartscherm, ondertussen verplicht op updates controleren
            SplashForm sp = new SplashForm();
            sp.Show();
            sp.Refresh();
            DateTime t0 = DateTime.Now;
            string upUrl = null, upErr = null;
            bool upAvail = false, upDone = false;
            DiscordGate.Result gateResult = DiscordGate.Result.NeedLogin;
            System.Threading.Thread uth = new System.Threading.Thread(delegate()
            {
                upAvail = Updater.UpdateAvailable(out upUrl, out upErr);
                if (!upAvail) gateResult = DiscordGate.Check(S);
                upDone = true;
            });
            uth.IsBackground = true;
            uth.Start();
            while (!upDone || (DateTime.Now - t0).TotalMilliseconds < 1400)
            {
                double el = (DateTime.Now - t0).TotalMilliseconds;
                sp.Progress = (float)Math.Min(upDone ? 1.0 : 0.85, el / 1400.0);
                if (!upDone && el > 1400) sp.Status = "Controleren op updates...";
                sp.Invalidate();
                Application.DoEvents();
                System.Threading.Thread.Sleep(15);
                if (el > 9000) break;
            }
            sp.Progress = 1f; sp.Refresh();
            System.Threading.Thread.Sleep(150);
            sp.Close(); sp.Dispose();
            if (upAvail) { Updater.ForceUpdate(upUrl); return; }

            // alleen voor leden van de Discord met de juiste rol
            if (gateResult != DiscordGate.Result.Ok)
            {
                using (AccessForm af = new AccessForm(S, gateResult))
                    if (af.ShowDialog() != DialogResult.OK) return;
            }
            if (string.IsNullOrEmpty(S.UserName) && !string.IsNullOrEmpty(DiscordGate.DiscordName))
            {
                S.UserName = DiscordGate.DiscordName.Length > 16 ? DiscordGate.DiscordName.Substring(0, 16) : DiscordGate.DiscordName;
                S.Save();
            }
            if (string.IsNullOrEmpty(S.UserName))
            {
                using (NameForm nf = new NameForm(""))
                    S.UserName = nf.ShowDialog() == DialogResult.OK ? nf.Result : "Speler";
                S.Save();
            }
            if (!Profiles.Exists(S.ActiveProfile))
            {
                List<string> existing = Profiles.Names();
                if (existing.Count > 0) { S.ActiveProfile = existing[0]; Profiles.Load(existing[0], S); }
                else Profiles.Save(S.ActiveProfile, S);
                S.Save();
            }
            overlay = new Overlay(S);
            menu = new MenuForm(S, overlay);

            overlay.Show();
            overlay.ApplySettings();
            ApplyStreamProof();

            Timer fgTimer = new Timer();
            fgTimer.Interval = 200;
            fgTimer.Tick += delegate { CheckForeground(); };
            fgTimer.Start();

            Timer rgbTimer = new Timer();
            rgbTimer.Interval = 33;
            rgbTimer.Tick += delegate
            {
                if (!S.Rgb) return;
                Crosshair.Hue = (Crosshair.Hue + S.RgbSpeed * 0.8f) % 360f;
                overlay.InvalidateCrosshair();
                if (menu.Visible) menu.InvalidatePreviews();
            };
            rgbTimer.Start();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += delegate
            {
                try { overlay.BeginInvoke(new Action(overlay.ApplySettings)); } catch { }
            };

            tray = new NotifyIcon();
            tray.Icon = AppIcon.MakeIcon(SystemInformation.SmallIconSize.Width);
            tray.Text = "Custom Crosshair";
            ContextMenuStrip cms = new ContextMenuStrip();
            cms.Items.Add("Menu openen", null, delegate { if (!menu.Visible) menu.Toggle(); });
            ToolStripMenuItem profMenu = new ToolStripMenuItem("Profiel");
            profMenu.DropDownItems.Add("...");
            cms.Items.Add(profMenu);
            cms.Opening += delegate
            {
                profMenu.DropDownItems.Clear();
                foreach (string n in Profiles.Names())
                {
                    string name = n;
                    ToolStripMenuItem it = new ToolStripMenuItem(name);
                    it.Checked = name == S.ActiveProfile;
                    it.Click += delegate { SwitchProfile(name); };
                    profMenu.DropDownItems.Add(it);
                }
            };
            cms.Items.Add("Crosshair aan/uit", null, delegate { S.Enabled = !S.Enabled; S.Save(); overlay.ApplySettings(); });
            cms.Items.Add("Afsluiten", null, delegate { Quit(); });
            tray.ContextMenuStrip = cms;
            tray.DoubleClick += delegate { menu.Toggle(); };
            tray.Visible = true;

            KeyHook.OnKeyDown = delegate(int vk)
            {
                overlay.BeginInvoke(new Action(delegate { HandleKey(vk); }));
            };
            KeyHook.Start();

            bool tour = !S.TourDone;
            if (S.ShowMenuOnStart || tour) menu.Toggle();
            if (S.LastSeenVersion != Updater.Version) { S.LastSeenVersion = Updater.Version; S.Save(); }
            if (tour) menu.BeginInvoke(new Action(menu.StartTour));
            Timer upd = new Timer();
            upd.Interval = 3 * 60 * 1000;
            upd.Tick += delegate { Updater.Check(overlay, false); };
            upd.Start();

            Application.Run(new ApplicationContext());
            KeyHook.Stop();
            GC.KeepAlive(mutex);
        }
    }
}