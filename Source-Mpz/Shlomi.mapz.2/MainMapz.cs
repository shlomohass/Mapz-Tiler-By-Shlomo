using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.Reflection;
using System.Device.Location;

namespace Shlomi.mapz._2
{
    public partial class MainMapz : Form
    {
        //Flags:
        bool update_map_on_combo_change = false;
        bool update_map_on_zoom_track_scroll = false;
        bool set_anchor_point_for_area_TL = false;
        bool set_anchor_point_for_area_BR = false;

        //Display:
        bool display_debuger = false;
        bool display_position = false;
        bool display_grid = false;

        // layers
        readonly GMapOverlay top = new GMapOverlay();
        internal readonly GMapOverlay objects = new GMapOverlay("objects");
        internal readonly GMapOverlay routes = new GMapOverlay("routes");
        public GMapOverlay polygons = new GMapOverlay("polygons");

        // marker
        GMapMarker currentMarker;

        public MainMapz()
        {
            InitializeComponent();

            //Set debuger:
            Debuger_intia();

            //Test Internet connection:
            if (!Stuff.PingNetwork("pingtest.com"))
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection available, going to CacheOnly mode.", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            // config map   
            MainMap.MapProvider = GMapProviders.List.Find(item => item.Name == Properties.Settings.Default.set_default_map) as GMapProvider;
            MainMap.Position = new PointLatLng(Properties.Settings.Default.def_lat, Properties.Settings.Default.def_lon);
            MainMap.CanDragMap = true;
            MainMap.MinZoom = 4;
            MainMap.MaxZoom = 20;
            MainMap.Zoom = Properties.Settings.Default.set_default_zoom;
            
            //Displays:
            this.display_debuger = Properties.Settings.Default.display_debuger;
            this.display_grid = Properties.Settings.Default.display_grid;
            this.display_position = Properties.Settings.Default.display_position;
            if (this.display_debuger) chk_display_debuger.Checked = true;
            if (this.display_grid) chk_display_grid.Checked = true;
            if (this.display_position) chk_display_position.Checked = true;
            this.chk_display_debuger_CheckedChanged(null, null);
            this.chk_display_grid_CheckedChanged(null, null);
            this.chk_display_position_CheckedChanged(null, null);

            //Load Map Types:
            cmb_mapType.ValueMember  = "Name";
            cmb_mapType.DataSource   = GMapProviders.List;
            cmb_mapType.SelectedItem = MainMap.MapProvider;

            //Track zoom:
            track_zoom.Minimum = MainMap.MinZoom;
            track_zoom.Maximum = MainMap.MaxZoom;
            numUpDown_zoom.Minimum = MainMap.MinZoom;
            numUpDown_zoom.Maximum = MainMap.MaxZoom;
            track_zoom.TickFrequency = 1;
            track_zoom.LargeChange = 1;
            track_zoom.SmallChange = 1;
            track_zoom.Value = (int)MainMap.Zoom;
            lbl_zoom.Text = MainMap.Zoom.ToString();
            track_zoom.Scroll += new System.EventHandler(this.track_zoom_Scroll_event);

            //Goto defaults:
            lbl_def_lat.Text = Properties.Settings.Default.def_lat.ToString();
            lbl_def_lon.Text = Properties.Settings.Default.def_lon.ToString();
            lbl_mouseposition_lat.Text = Properties.Settings.Default.def_lat.ToString();
            lbl_mouseposition_lon.Text = Properties.Settings.Default.def_lon.ToString();

            // add custom layers  
            {
                MainMap.Overlays.Add(routes);
                MainMap.Overlays.Add(polygons);
                MainMap.Overlays.Add(objects);
                MainMap.Overlays.Add(top);

                //routes.Routes.CollectionChanged += new GMap.NET.ObjectModel.NotifyCollectionChangedEventHandler(Routes_CollectionChanged);
                //objects.Markers.CollectionChanged += new GMap.NET.ObjectModel.NotifyCollectionChangedEventHandler(Markers_CollectionChanged);
            }

            // set current marker
            this.currentMarker = new GMarkerGoogle(MainMap.Position, GMarkerGoogleType.arrow);
            this.currentMarker.IsHitTestVisible = false;
            this.top.Markers.Add(currentMarker);

            //Load prev areas:
            txt_area_select_name.Text = "Area-" + (polygons.Polygons.Count + 1).ToString();


            //Enable Reactions:
            this.update_map_on_combo_change = true;
            this.update_map_on_zoom_track_scroll = true;

            {
                //MainMap.OnPositionChanged += new PositionChanged(MainMap_OnPositionChanged);

                //MainMap.OnTileLoadStart += new TileLoadStart(MainMap_OnTileLoadStart);
                //MainMap.OnTileLoadComplete += new TileLoadComplete(MainMap_OnTileLoadComplete);

                MainMap.OnMapZoomChanged += new MapZoomChanged(MainMap_OnMapZoomChanged);
                MainMap.OnMapTypeChanged += new MapTypeChanged(MainMap_OnMapTypeChanged);

                //MainMap.OnMarkerClick += new MarkerClick(MainMap_OnMarkerClick);
                //MainMap.OnMarkerEnter += new MarkerEnter(MainMap_OnMarkerEnter);
                //MainMap.OnMarkerLeave += new MarkerLeave(MainMap_OnMarkerLeave);

                //MainMap.OnPolygonEnter += new PolygonEnter(MainMap_OnPolygonEnter);
                //MainMap.OnPolygonLeave += new PolygonLeave(MainMap_OnPolygonLeave);

                //MainMap.OnRouteEnter += new RouteEnter(MainMap_OnRouteEnter);
                //MainMap.OnRouteLeave += new RouteLeave(MainMap_OnRouteLeave);

                MainMap.Manager.OnTileCacheComplete += new TileCacheComplete(OnTileCacheComplete);
                MainMap.Manager.OnTileCacheStart += new TileCacheStart(OnTileCacheStart);
                MainMap.Manager.OnTileCacheProgress += new TileCacheProgress(OnTileCacheProgress);
            }   

        }

        #region -- map events --

        private void MainMap_OnMapZoomChanged()
        {
            track_zoom.Value = (int)MainMap.Zoom;
            lbl_zoom.Text = MainMap.Zoom.ToString();
        }

        private void MainMap_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            double X = MainMap.FromLocalToLatLng(e.X, e.Y).Lng;
            double Y = MainMap.FromLocalToLatLng(e.X, e.Y).Lat;

            string longitude = X.ToString();
            string latitude = Y.ToString();
            lbl_mouseposition_lat.Text = latitude;
            lbl_mouseposition_lon.Text = longitude;
        }

        private void MainMap_OnMapTypeChanged(GMapProvider type)
        {
            track_zoom.Minimum = MainMap.MinZoom;
            track_zoom.Maximum = MainMap.MaxZoom;
            numUpDown_zoom.Minimum = MainMap.MinZoom;
            numUpDown_zoom.Maximum = MainMap.MaxZoom;
            track_zoom.Value = (int)MainMap.Zoom;
            lbl_zoom.Text = track_zoom.Value.ToString();
        }

        private void MainMap_Click(object sender, MouseEventArgs e)
        {
            base.OnMouseClick(e);
            double X = MainMap.FromLocalToLatLng(e.X, e.Y).Lng;
            double Y = MainMap.FromLocalToLatLng(e.X, e.Y).Lat;

            string longitude = X.ToString();
            string latitude = Y.ToString();
            Move_Main_marker_onclick(new PointLatLng(Y,X));

            if (this.set_anchor_point_for_area_TL)
            {
                txt_parser_TL_lat.Text = latitude;
                txt_parser_TL_lon.Text = longitude;
                this.btn_parser_TL_point_select_Click(null, null);
            }
            if (this.set_anchor_point_for_area_BR)
            {
                txt_parser_BR_lat.Text = latitude;
                txt_parser_BR_lon.Text = longitude;
                this.btn_parser_BR_point_select_Click(null, null);
            }
        }

        private void MainMap_DoubleClick(object sender, EventArgs e)
        {
            
        }

        private void Move_Main_marker_onclick(PointLatLng point)
        {
            this.currentMarker.Position = point;
        }

        
        void OnTileCacheComplete()
        {
            long size = 0;
            int db = 0;
            try
            {
                DirectoryInfo di = new DirectoryInfo(MainMap.CacheLocation);
                var dbs = di.GetFiles("*.gmdb", SearchOption.AllDirectories);
                foreach (var d in dbs)
                {
                    size += d.Length;
                    db++;
                }
            }
            catch
            {
            }

            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    
                    string text_toAdd = string.Format(CultureInfo.InvariantCulture, "{0} db in {1:00} MB", db, size / (1024.0 * 1024.0));
                    lbl_cache_status.Text = "all tiles saved! -> On disk : " + text_toAdd;
                };

                if (!IsDisposed)
                {
                    try
                    {
                        Invoke(m);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        void OnTileCacheStart()
        {

            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    lbl_cache_status.Text = "saving tiles...";
                };
                Invoke(m);
            }
        }

        void OnTileCacheProgress(int left)
        {


            if (!IsDisposed)
            {
                MethodInvoker m = delegate
                {
                    lbl_cache_status.Text = left + " tiles to save...";
                };
                Invoke(m);
            }
        }

        #endregion

        #region -- ui events Tab tools --

        private void cmb_mapType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.update_map_on_combo_change)
            {
                if (debuger) this.Debuger_push(new string[] { "Changing Map", "Target Map:" + cmb_mapType.SelectedText });
                MainMap.MapProvider = cmb_mapType.SelectedItem as GMapProvider;
            }
        }

        private void reload_map_Click(object sender, EventArgs e)
        {
            if (debuger) this.Debuger_push(new string[] { "Reload Map clicked", "Calling reload procedure" });
            MainMap.ReloadMap();
        }

        private void btn_load_def_map_Click(object sender, EventArgs e)
        {
            if (debuger) this.Debuger_push(new string[] { "Load Map default", "Target Map:" + Properties.Settings.Default.set_default_map });
            MainMap.MapProvider = GMapProviders.List.Find(item => item.Name == Properties.Settings.Default.set_default_map) as GMapProvider;
        }

        private void btn_set_default_map_Click(object sender, EventArgs e)
        {
            if (debuger) this.Debuger_push(new string[] { "Save Map default", "Target Map:" + MainMap.MapProvider.Name });
            Properties.Settings.Default.set_default_map = MainMap.MapProvider.Name;
            Properties.Settings.Default.Save();
            MessageBox.Show("Saved Defualt Map Type", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void track_zoom_Scroll_event(object sender, System.EventArgs e)
        {
            if (this.update_map_on_zoom_track_scroll && MainMap.Zoom != track_zoom.Value)
            {
                MainMap.Zoom = track_zoom.Value;
            }
        }

        private void clear_debuger_click(object sender, EventArgs e)
        {
            this.Debuger_clean();
        }

        private void btn_goto_Click(object sender, EventArgs e)
        {
            
            double lat = double.Parse(lbl_def_lat.Text, CultureInfo.InvariantCulture);
            double lon = double.Parse(lbl_def_lon.Text, CultureInfo.InvariantCulture);
            double Mlat = MainMap.Position.Lat;
            double Mlon = MainMap.Position.Lng;
            if (debuger) this.Debuger_push(new string[] { "Position jump", "Changing position: lat-" + lat.ToString() + " / lon-" + lon.ToString() });
            if (lat != Mlat || lon != Mlon)
            {
                MainMap.Position = new PointLatLng(lat, lon);
                lbl_mouseposition_lat.Text = lat.ToString();
                lbl_mouseposition_lon.Text = lon.ToString();
            }
        }

        private void btn_set_coord_def_Click(object sender, EventArgs e)
        {
            double lat = double.Parse(lbl_def_lat.Text, CultureInfo.InvariantCulture);
            double lon = double.Parse(lbl_def_lon.Text, CultureInfo.InvariantCulture);
            if (debuger) this.Debuger_push(new string[] { "Save Position default", "Target Coords: lat-" + lat.ToString() + " / lon-" + lon.ToString() });
            Properties.Settings.Default.def_lat = lat;
            Properties.Settings.Default.def_lon = lon;
            Properties.Settings.Default.Save();
            MessageBox.Show("Saved Defualt Coordinates", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btn_get_goto_from_map_Click(object sender, EventArgs e)
        {
            string Mlat = MainMap.Position.Lat.ToString();
            string Mlon = MainMap.Position.Lng.ToString();
            if (debuger) this.Debuger_push(new string[] { "Get Position Coordinates", "Target Coords: lat-" + Mlat + " / lon-" + Mlon });
            lbl_def_lat.Text = Mlat;
            lbl_def_lon.Text = Mlon;
        }

        private void chk_display_debuger_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_display_debuger.Checked)
            {
                if (debuger) this.Debuger_push(new string[] { "Debuger Display", "Exposed Debuger" });
                this.display_debuger = true;
                grp_debuger_view.Visible = true;
            }
            else
            {
                if (debuger) this.Debuger_push(new string[] { "Debuger Display", "Hide Debuger" });
                this.display_debuger = false;
                grp_debuger_view.Visible = false;
            }
        }

        private void chk_display_position_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_display_position.Checked)
            {
                if (debuger) this.Debuger_push(new string[] { "Position Display", "Exposed Position" });
                this.display_position = true;
                grp_position_view.Visible = true;
            }
            else
            {
                if (debuger) this.Debuger_push(new string[] { "Position Display", "Hide Position" });
                this.display_position = false;
                grp_position_view.Visible = false;
            }
        }

        private void chk_display_grid_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_display_grid.Checked)
            {
                if (debuger) this.Debuger_push(new string[] { "Grid Display", "Exposed Grid" });
                this.display_grid = true;
                MainMap.ShowTileGridLines = true;
            }
            else
            {
                if (debuger) this.Debuger_push(new string[] { "Grid Display", "Hide Grid" });
                this.display_grid = false;
                MainMap.ShowTileGridLines = false;
            }
        }

        private void btn_set_default_view_Click(object sender, EventArgs e)
        {
            if (chk_display_grid.Checked)
            {
                Properties.Settings.Default.display_grid = true;
            }
            else
            {
                Properties.Settings.Default.display_grid = false;
            }
            if (chk_display_position.Checked)
            {
                Properties.Settings.Default.display_position = true;
            }
            else
            {
                Properties.Settings.Default.display_position = false;
            }
            if (chk_display_debuger.Checked)
            {
                Properties.Settings.Default.display_debuger = true;
            }
            else
            {
                Properties.Settings.Default.display_debuger = false;
            }
            Properties.Settings.Default.Save();
            if (debuger) this.Debuger_push(new string[] { "Default View", "Save Settings" });
            MessageBox.Show("Saved View Settings", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region -- debuger --

        public bool debuger = false;
        public void Debuger_intia()
        {
            this.debuger = Properties.Settings.Default.debuger;
            if (this.debuger) Debuger_expose();
            else Debuger_hide();
        }
        public void Debuger_expose()
        {
            grp_debuger_view.Visible = true;
        }
        public void Debuger_hide()
        {
            grp_debuger_view.Visible = false;
        }
        public void Debuger_push(string[] row)
        {
            //{ textBox1.Text, textBox2.Text, textBox3.Text }
            if (debuger_list.Items.Count > 150) Debuger_clean();
            debuger_list.Items.Add(new ListViewItem(row));
            debuger_list.EnsureVisible(debuger_list.Items.Count - 1);
        }
        public void Debuger_clean()
        {
            debuger_list.Items.Clear();
        }
        #endregion

        #region -- ui events Tab parser --

        private void btn_area_from_selection_Click(object sender, EventArgs e)
        {
            /*
            MainMap.SelectedArea = new RectLatLng(
                new PointLatLng(Properties.Settings.Default.def_lat, Properties.Settings.Default.def_lon),
                new SizeLatLng(200.0,200.0)
            );
             */
            RectLatLng area = MainMap.SelectedArea;
            if (area.IsEmpty)
            {
                if (debuger) this.Debuger_push(new string[] { "Area Creation", "Reject - no selection" });
                MessageBox.Show("Select map area holding ALT key", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            
            PointLatLng selectedTL = MainMap.SelectedArea.LocationTopLeft;
            PointLatLng selectedTR = new PointLatLng(MainMap.SelectedArea.LocationTopLeft.Lat, MainMap.SelectedArea.LocationRightBottom.Lng);
            PointLatLng selectedBR = MainMap.SelectedArea.LocationRightBottom;
            PointLatLng selectedBL = new PointLatLng(MainMap.SelectedArea.LocationRightBottom.Lat, MainMap.SelectedArea.LocationTopLeft.Lng);
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(selectedTL);
            points.Add(selectedTR);
            points.Add(selectedBR);
            points.Add(selectedBL);
            
            string name = txt_area_select_name.Text;
            if (string.IsNullOrWhiteSpace(txt_area_select_name.Text)) { name = "Area-" + (polygons.Polygons.Count + 1).ToString(); }

            GMapPolygon polygon = new GMapPolygon(points, name);
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polygons.Polygons.Add(polygon);

            txt_area_select_name.Text = "Area-" + (polygons.Polygons.Count + 1).ToString();

            fill_areas(polygon);
            if (debuger) this.Debuger_push(new string[] { "Area Creation", "Created - from selection" });
        }

        private void btn_parser_TL_point_select_Click(object sender, EventArgs e)
        {
            if (!this.set_anchor_point_for_area_TL)
            {
                this.set_anchor_point_for_area_BR = false;
                this.set_anchor_point_for_area_TL = true;
                btn_parser_TL_point_select.BackColor = SystemColors.Highlight;
                this.Cursor = Cursors.Cross;
            }
            else
            {
                this.set_anchor_point_for_area_BR = false;
                this.set_anchor_point_for_area_TL = false;
                btn_parser_TL_point_select.BackColor = Color.Transparent;
                btn_parser_BR_point_select.BackColor = Color.Transparent;
                this.Cursor = Cursors.Default;
            }
        }

        private void btn_parser_BR_point_select_Click(object sender, EventArgs e)
        {
            if (!this.set_anchor_point_for_area_BR)
            {
                this.set_anchor_point_for_area_TL = false;
                this.set_anchor_point_for_area_BR = true;
                btn_parser_BR_point_select.BackColor = SystemColors.Highlight;
                this.Cursor = Cursors.Cross;
            }
            else
            {
                this.set_anchor_point_for_area_BR = false;
                this.set_anchor_point_for_area_TL = false;
                btn_parser_TL_point_select.BackColor = Color.Transparent;
                btn_parser_BR_point_select.BackColor = Color.Transparent;
                this.Cursor = Cursors.Default;
            }
        }

        private void btn_manual_add_area_Click(object sender, EventArgs e)
        {
            double Lat_TL = 0.0;
            double lon_TL = 0.0;
            double Lat_BR = 0.0;
            double lon_BR = 0.0;
            if (
                   string.IsNullOrWhiteSpace(txt_parser_TL_lat.Text)
                || string.IsNullOrWhiteSpace(txt_parser_TL_lon.Text)
                || string.IsNullOrWhiteSpace(txt_parser_BR_lat.Text)
                || string.IsNullOrWhiteSpace(txt_parser_BR_lon.Text)
                || !Double.TryParse(txt_parser_TL_lat.Text, out Lat_TL)
                || !Double.TryParse(txt_parser_TL_lon.Text, out lon_TL)
                || !Double.TryParse(txt_parser_BR_lat.Text, out Lat_BR)
                || !Double.TryParse(txt_parser_BR_lon.Text, out lon_BR)
            )
            {
                if (debuger) this.Debuger_push(new string[] { "Area Creation", "Reject - not valid manual coords" });
                MessageBox.Show("Set valid coordinates in TL / BR", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PointLatLng selectedTL = new PointLatLng(Lat_TL, lon_TL);
            PointLatLng selectedTR = new PointLatLng(Lat_TL, lon_BR);
            PointLatLng selectedBR = new PointLatLng(Lat_BR, lon_BR);
            PointLatLng selectedBL = new PointLatLng(Lat_BR, lon_TL);
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(selectedTL);
            points.Add(selectedTR);
            points.Add(selectedBR);
            points.Add(selectedBL);

            string name = txt_area_select_name.Text;
            if (string.IsNullOrWhiteSpace(txt_area_select_name.Text)) { name = "Area-" + (polygons.Polygons.Count + 1).ToString(); }


            GMapPolygon polygon = new GMapPolygon(points,name);
            polygon.Fill = new SolidBrush(Color.FromArgb(50, Color.Red));
            polygon.Stroke = new Pen(Color.Red, 1);
            polygons.Polygons.Add(polygon);

            txt_area_select_name.Text = "Area-" + (polygons.Polygons.Count + 1).ToString();
            txt_parser_TL_lat.Text    = "";
            txt_parser_TL_lon.Text    = "";
            txt_parser_BR_lat.Text    = "";
            txt_parser_BR_lon.Text    = "";

            fill_areas(polygon);
            if (debuger) this.Debuger_push(new string[] { "Area Creation", "Created - from manual coords" });
        }

        private void btn_center_to_area_Click(object sender, EventArgs e)
        {
            GMapPolygon areaname = cmb_areas.SelectedValue as GMapPolygon;
            if (areaname != null)
            {
                if (debuger) this.Debuger_push(new string[] { "Center Map", "Center To area : " + areaname.Name });
                MainMap.Position = this.calc_rectCenter(areaname);
            }
            else
            {
                if (debuger) this.Debuger_push(new string[] { "Center Map", "No Area To center" });
            }
        }

        private void btn_area_remove_Click(object sender, EventArgs e)
        {
            GMapPolygon areaname = cmb_areas.SelectedValue as GMapPolygon;
            if (areaname != null)
            {
                if (debuger) this.Debuger_push(new string[] { "Reamove Area", "Area removed : " + areaname.Name });
                polygons.Polygons.Remove(areaname);
                fill_areas();
            }
            else
            {
                if (debuger) this.Debuger_push(new string[] { "Reamove Area", "No Area to remove" });
            }
        }

        private void btn_area_removeAll_Click(object sender, EventArgs e)
        {
            polygons.Polygons.Clear();
            fill_areas();
            txt_area_select_name.Text = "Area-" + (polygons.Polygons.Count + 1).ToString();
            if (debuger) this.Debuger_push(new string[] { "Reamove All Areas", "Removed All" });
        }

        private void btn_capture_area_Click(object sender, EventArgs e)
        {
            
            GMapPolygon areaname = cmb_areas.SelectedValue as GMapPolygon;
            if (areaname != null)
            {
                if (debuger) this.Debuger_push(new string[] { "Capture Area", "Luanch" });
                Capture st = new Capture(this, areaname, (int)numUpDown_zoom.Value, (int)numUpDown_cols.Value, (int)numUpDown_rows.Value);
                st.Owner = this;
                st.Show();
            }
            else
            {
                MessageBox.Show("Select Area to parse first", "Mapz 1.0.1 - Shlomo hassid", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                if (debuger) this.Debuger_push(new string[] { "Capture Area", "No Area Selected" });
            }
        }

        private void btn_set_current_zoom_Click(object sender, EventArgs e)
        {
            if (debuger) this.Debuger_push(new string[] { "Set zoom target", "Copiep Zoom From Current view" });
            numUpDown_zoom.Value = (int)MainMap.Zoom;
        }
        #endregion

        #region   -- functions --

        public void fill_areas(GMapPolygon selectedP = null)
        {
            Dictionary<string, GMapPolygon> comboSource = new Dictionary<string, GMapPolygon>();
            foreach (GMapPolygon poly in polygons.Polygons)
            {
                comboSource.Add(poly.Name, poly);
            }
            if (comboSource.Count == 0)
            {
                cmb_areas.DataSource = null;
                return;
            }
            cmb_areas.DataSource = new BindingSource(comboSource, null);
            cmb_areas.DisplayMember = "Key";
            cmb_areas.ValueMember = "Value";
            if (selectedP != null) cmb_areas.SelectedValue = selectedP;
        }

        public PointLatLng calc_rectCenter(GMapPolygon target) 
        {
            List<GeoCoordinate> collectPoints = new List<GeoCoordinate>();
            foreach (PointLatLng point in target.Points)
            {
                collectPoints.Add(new GeoCoordinate(point.Lat, point.Lng));
            }
            GeoCoordinate centerPoint = GetCentralGeoCoordinate(collectPoints);
            return new PointLatLng(centerPoint.Latitude, centerPoint.Longitude);
        }
        public GeoCoordinate GetCentralGeoCoordinate(List<GeoCoordinate> geoCoordinates)
        {
            if (geoCoordinates.Count == 1)
            {
                return geoCoordinates[0];
            }

            double x = 0;
            double y = 0;
            double z = 0;

            foreach (var geoCoordinate in geoCoordinates)
            {
                var latitude = geoCoordinate.Latitude * Math.PI / 180;
                var longitude = geoCoordinate.Longitude * Math.PI / 180;

                x += Math.Cos(latitude) * Math.Cos(longitude);
                y += Math.Cos(latitude) * Math.Sin(longitude);
                z += Math.Sin(latitude);
            }

            var total = geoCoordinates.Count;

            x = x / total;
            y = y / total;
            z = z / total;

            var centralLongitude = Math.Atan2(y, x);
            var centralSquareRoot = Math.Sqrt(x * x + y * y);
            var centralLatitude = Math.Atan2(z, centralSquareRoot);

            return new GeoCoordinate(centralLatitude * 180 / Math.PI, centralLongitude * 180 / Math.PI);
        }

        #endregion

    }
}
