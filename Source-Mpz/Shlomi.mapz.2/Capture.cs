using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using GMap.NET.MapProviders;

namespace Shlomi.mapz._2
{
    public partial class Capture : Form
    {
        MainMapz Main;

        BackgroundWorker bg = new BackgroundWorker();
        readonly List<GPoint> tileArea = new List<GPoint>();
        private GMapPolygon Area;
        private List<SubRect> Sub_areas = new List<SubRect>();
        private GMapProvider MapType;
        private int Zoom;
        private int Cols;
        private int Rows;
        private int TotTiles = 1;
        private int Doing    = 0;
        private string sPath;
        private bool Addscale;
        private bool Addcoords;

        bool Run  = false; 

        public Capture(MainMapz main, GMapPolygon area, int zoom , int cols, int rows)
        {
            InitializeComponent();

            Main = main;
            Zoom = zoom;
            Cols = cols;
            Rows = rows;
            Area = area;
            TotTiles = Cols * Rows;
            MapType = Main.MainMap.MapProvider;

            lbl_Doing.Text = Doing.ToString();
            lbl_Tot.Text = TotTiles.ToString();
            lbl_capture_area_name.Text = area.Name;
            panel_satuts_capturing.Visible = false;
            txt_folder_path.Text = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

            bg.WorkerReportsProgress = true;
            bg.WorkerSupportsCancellation = true;
            bg.DoWork += new DoWorkEventHandler(bg_DoWork);
            bg.ProgressChanged += new ProgressChangedEventHandler(bg_ProgressChanged);
            bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    MessageBox.Show("Error:" + e.Error.ToString(), "Shlomo Mapz", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (e.Result != null)
                {
                    try
                    {
                        if (this.chk_open_results.Checked)
                        {
                            Process.Start(e.Result as string);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            this.Doing++;

            if (this.Doing < this.TotTiles)
            {
                this.Text = "Mapz Capture Procedure";
                progressBar1.Value = 0;
                this.continue_job();
            }
            else
            {
                this.Text = "Mapz Capture Done";
                progressBar1.Value = 0;
                MessageBox.Show("Job Done!", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }

        }

        void bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            GPoint p = (GPoint)e.UserState;
            this.Text = "Mapz Capture: Downloading[" + p + "]: " + tileArea.IndexOf(p) + " of " + tileArea.Count;
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            MapInfo info = (MapInfo)e.Argument;
            if (!info.Area.subArea.IsEmpty)
            {
                string bigImage = info.strPath + Path.DirectorySeparatorChar + "Mapz-" + info.AreaName + " - Index-c" + info.Area.colIndex + "r" + info.Area.rowIndex + " - zoom-" + info.Zoom + " - Map type-" + Stuff.ReplaceInvalidFileNameChars(info.Type.Name,"") + ".png";
                   
                e.Result = bigImage;

                // current area
                GPoint topLeftPx = info.Type.Projection.FromLatLngToPixel(info.Area.subArea.LocationTopLeft, info.Zoom);
                GPoint rightButtomPx = info.Type.Projection.FromLatLngToPixel(info.Area.subArea.Bottom, info.Area.subArea.Right, info.Zoom);
                GPoint pxDelta = new GPoint(rightButtomPx.X - topLeftPx.X, rightButtomPx.Y - topLeftPx.Y);
                GMap.NET.GSize maxOfTiles = info.Type.Projection.GetTileMatrixMaxXY(info.Zoom);

                //Set padding
                int padding = 22;
                if (!info.addAscale && !info.addCoords) { padding = 0; }

                
                using (Bitmap bmpDestination = new Bitmap((int)(pxDelta.X + padding * 2), (int)(pxDelta.Y + padding * 2)))
                {
                    using (Graphics gfx = Graphics.FromImage(bmpDestination))
                    {
                        gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        gfx.SmoothingMode = SmoothingMode.HighQuality;

                        int i = 0;

                        // get tiles & combine into one
                        lock (tileArea)
                        {
                            foreach (var p in tileArea)
                            {
                                if (bg.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                int pc = (int)(((double)++i / tileArea.Count) * 100);
                                bg.ReportProgress(pc, p);

                                foreach (var tp in info.Type.Overlays)
                                {
                                    Exception ex;
                                    GMapImage tile;

                                    // tile number inversion(BottomLeft -> TopLeft) for pergo maps
                                    if (tp.InvertedAxisY)
                                    {
                                        tile = GMaps.Instance.GetImageFrom(tp, new GPoint(p.X, maxOfTiles.Height - p.Y), info.Zoom, out ex) as GMapImage;
                                    }
                                    else // ok
                                    {
                                        tile = GMaps.Instance.GetImageFrom(tp, p, info.Zoom, out ex) as GMapImage;
                                    }

                                    if (tile != null)
                                    {
                                        using (tile)
                                        {
                                            long x = p.X * info.Type.Projection.TileSize.Width - topLeftPx.X + padding;
                                            long y = p.Y * info.Type.Projection.TileSize.Width - topLeftPx.Y + padding;
                                            {
                                                gfx.DrawImage(tile.Img, x, y, info.Type.Projection.TileSize.Width, info.Type.Projection.TileSize.Height);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // draw info
                        if (info.addAscale || info.addCoords)
                        {
                            System.Drawing.Rectangle rect = new System.Drawing.Rectangle();
                            {
                                rect.Location = new System.Drawing.Point(padding, padding);
                                rect.Size = new System.Drawing.Size((int)pxDelta.X, (int)pxDelta.Y);
                            }

                            using (Font f = new Font(FontFamily.GenericSansSerif, 8, FontStyle.Bold))
                            {
                                if (info.addCoords)
                                {
                                    // draw bounds & coordinates
                                    using (Pen p = new Pen(Brushes.Blue, 1))
                                    {
                                        p.DashStyle = System.Drawing.Drawing2D.DashStyle.DashDot;

                                        gfx.DrawRectangle(p, rect);

                                        string topleft = info.Area.subArea.LocationTopLeft.ToString();
                                        SizeF s = gfx.MeasureString(topleft, f);

                                        gfx.DrawString(topleft, f, p.Brush, rect.X + s.Height / 2, rect.Y + s.Height / 2);

                                        string rightBottom = new PointLatLng(info.Area.subArea.Bottom, info.Area.subArea.Right).ToString();
                                        SizeF s2 = gfx.MeasureString(rightBottom, f);

                                        gfx.DrawString(rightBottom, f, p.Brush, rect.Right - s2.Width - s2.Height / 2, rect.Bottom - s2.Height - s2.Height / 2);
                                    }
                                }
                                if (info.addAscale)
                                {
                                    // draw scale
                                    using (Pen p = new Pen(Brushes.Blue, 1))
                                    {
                                        double rez = info.Type.Projection.GetGroundResolution(info.Zoom, info.Area.subArea.Bottom);
                                        int px100 = (int)(100.0 / rez); // 100 meters
                                        int px1000 = (int)(1000.0 / rez); // 1km   

                                        gfx.DrawRectangle(p, rect.X + 10, rect.Bottom - 20, px1000, 10);
                                        gfx.DrawRectangle(p, rect.X + 10, rect.Bottom - 20, px100, 10);

                                        string leftBottom = "scale: 100m | 1Km";
                                        SizeF s = gfx.MeasureString(leftBottom, f);
                                        gfx.DrawString(leftBottom, f, p.Brush, rect.X + 10, rect.Bottom - s.Height - 20);
                                    }
                                }
                            }
                        }
                    }

                    bmpDestination.Save(bigImage, ImageFormat.Png);
                    bmpDestination.Dispose();
                }
            }
        }

        readonly List<PointLatLng> GpxRoute = new List<PointLatLng>();
        RectLatLng AreaGpx = RectLatLng.Empty;

        public void start_job()
        {
            if (!bg.IsBusy && !this.Run)
            {
                Run = true;
                RectLatLng rect = RectParse.parse(Area);
                Sub_areas = RectParse.generate_subs(rect, Cols, Rows);
                SubRect area = Sub_areas[this.Doing];
                progressBar1.Value = 0;
                lock (tileArea)
                {
                    tileArea.Clear();
                    tileArea.AddRange(MapType.Projection.GetAreaTileList(area.subArea, (int)Zoom, 1));
                    tileArea.TrimExcess();
                }
                bg.RunWorkerAsync(new MapInfo(area, this.Zoom, this.MapType, this.Addscale, this.Addcoords, this.Area.Name, this.sPath));
                
            }
        }

        public void continue_job()
        {
            if (!bg.IsBusy && this.Run)
            {
                SubRect area = Sub_areas[this.Doing];
                lbl_Doing.Text = (this.Doing + 1).ToString();
                lock (tileArea)
                {
                    tileArea.Clear();
                    tileArea.AddRange(MapType.Projection.GetAreaTileList(area.subArea, (int)Zoom, 1));
                    tileArea.TrimExcess();
                }
                bg.RunWorkerAsync(new MapInfo(area, this.Zoom, this.MapType, this.Addscale, this.Addcoords, this.Area.Name, this.sPath));
            }
            else
            {
                MessageBox.Show("All Canceled", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
            }
        }

        private void cancel_job_Click(object sender, EventArgs e)
        {
            if (bg.IsBusy)
            {
                bg.CancelAsync();
            }
        }

        private void btn_cancel_all_Click(object sender, EventArgs e)
        {
            this.Run = false;
            this.cancel_job_Click(null, null);
        }

        public struct MapInfo
        {
            public SubRect Area;
            public int Zoom;
            public GMapProvider Type;
            public bool addAscale;
            public bool addCoords;
            public string AreaName;
            public string strPath;

            public MapInfo(SubRect area, int Zoom, GMapProvider Type, bool addAscale, bool addCoords, string name, string path)
            {
                this.Area = area;
                this.Zoom = Zoom;
                this.Type = Type;
                this.addAscale = addAscale;
                this.addCoords = addCoords;
                this.AreaName = name;
                this.strPath = path;
            }
        }

        private void btn_set_target_folder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string savePath = folderBrowserDialog1.SelectedPath;
                txt_folder_path.Text = savePath;
            }
        }

        private void btn_strat_job_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_folder_path.Text))
            {
                this.sPath = txt_folder_path.Text;
                this.Addcoords = false;
                this.Addscale = false;
                if (this.chk_add_scale.Checked)
                {
                    this.Addscale = true;
                }
                if (this.chk_add_coords.Checked)
                {
                    this.Addcoords = true;
                }
                lbl_Doing.Text = (this.Doing + 1).ToString();
                this.panel_satuts_capturing.Visible = true;
                this.btn_strat_job.Enabled = false;
                this.btn_set_target_folder.Enabled = false;

                this.start_job();
            }
            else
            {
                MessageBox.Show("Set valid Folder Path Before dtarting the operation", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }


    
    }
}
