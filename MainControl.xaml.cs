using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WPFUI.Controls;

namespace McZiper4ObserverBot
{
    /// <summary>
    /// MainControl.xaml の相互作用ロジック
    /// </summary>
    public partial class MainControl : UserControl
    {
        private string _WorldDataPath;
        private List<string> _PluginPath = new List<string>();
        private List<string> _ModPath = new List<string>();
        public MainControl()
        {
            InitializeComponent();
            this.WorldDataBtn.Click += OnWorldDataBtnClick;
            this.PluginAddBtn.Click += OnPluginAddBtnClick;
            this.ModAddBtn.Click += OnModAddBtnClick;
            this.PropAddBtn.Click += OnPropAddBtnClick;
            this.PropsBox.Loaded += OnPropsBoxLoaded;
            this.CreateZipBtn.Click += OnCreateZipBtnClick;
        }

        private void UpdatedValue()
        {
            foreach(CardExpander ce in PropsBox.Children)
            {
                if (PropertiesService.Properties[ce.Header.ToString()].Value == null)
                {
                    PropertiesService.Properties[ce.Header.ToString()].Value
                        = PropertiesService.Properties[ce.Header.ToString()].InitialValue;
                }
                if((string)ce.Tag == "bool")
                {
                    ((ToggleSwitch)ce.HeaderContent).IsChecked
                        = bool.Parse(PropertiesService.Properties[ce.Header.ToString()].Value);
                }
                if ((string)ce.Tag == "string")
                {
                    ((WPFUI.Controls.TextBox)ce.HeaderContent).Text
                        = PropertiesService.Properties[ce.Header.ToString()].Value;
                }
                if ((string)ce.Tag == "decimal")
                {
                    ((NumberBox)ce.HeaderContent).Text
                        = PropertiesService.Properties[ce.Header.ToString()].Value;
                }
                if ((string)ce.Tag == "enum")
                {
                    ((ComboBox)ce.HeaderContent).SelectedItem
                        = PropertiesService.Properties[ce.Header.ToString()].Value;
                }
            }
            UpdateContent();
        }
        private void UpdateContent()
        {
            foreach (CardExpander ce in PropsBox.Children)
            {
               ((TextBlock)ce.Content).Text
                    = ce.Header + "=" + PropertiesService.Properties[ce.Header.ToString()].Value;
            }
        }

        private void OnPropAddBtnClick(object sender, RoutedEventArgs e)
        {
            string s = GetFileFromDialog(false, ".properties", false)[0];
            PropertiesService.ServerPropertiesFileToPropertiesDictionary(s);
            UpdatedValue();
        }

        private void OnCreateZipBtnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_WorldDataPath))
            {
                if (Path.GetFileName(_WorldDataPath) != 
                    PropertiesService.Properties["level-name"].Value)
                {
                    PropertiesService.Properties["level-name"].Value
                        = Path.GetFileName(_WorldDataPath);
                }
            }
            using (CommonSaveFileDialog sfd = new CommonSaveFileDialog())
            {
                sfd.Filters.Add(new CommonFileDialogFilter(".zip", ".zip"));
                if(sfd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    CreateZipService.CreateZip(
                        _WorldDataPath, _PluginPath, _ModPath, sfd.FileName);
                }
            }
            
            
        }

        private void OnPropsBoxLoaded(object sender, RoutedEventArgs e)
        {
            var ps = PropertiesService.Properties.ToList();
            foreach(var p in ps)
            {
                CardExpander ce = new CardExpander();
                ce.Header = p.Key;
                ce.Subtitle = p.Value.ExplainSentense;
                ce.Margin = new Thickness(0, 0, 0, 10);
                if(p.Value.ValueType == typeof(bool))
                {
                    ToggleSwitch ts = new ToggleSwitch();
                    ts.Margin = new Thickness(0, 0, 10, 0);
                    ce.Tag = "bool";
                    ce.HeaderContent = ts;
                    ts.Click += (o, ee) =>
                    {
                        p.Value.Value = ts.IsChecked.ToString().ToLower();
                        UpdateContent();
                    };
                }
                if(p.Value.ValueType == typeof(string))
                {
                    WPFUI.Controls.TextBox tb = new WPFUI.Controls.TextBox();
                    
                    tb.Width = 200;
                    tb.Height = 35;
                    tb.Margin = new Thickness(0, 0, 10, 0);
                    ce.Tag = "string";
                    ce.HeaderContent = tb;   
                    tb.TextChanged += (o, ee) =>
                    {
                        p.Value.Value = tb.Text;
                        UpdateContent();
                    };
                }
                if (p.Value.ValueType == typeof(uint) ||
                    p.Value.ValueType == typeof(ulong))
                {
                    NumberBox nb = new NumberBox();
                    nb.Margin = new Thickness(0, 0, 10, 0);
                    nb.Width = 135;
                    nb.Height = 35;
                    nb.IntegersOnly = true;
                    nb.Step = 1;
                    ce.Tag = "decimal";
                    ce.HeaderContent = nb;
                    nb.TextChanged += (o, ee) => 
                    {
                        p.Value.Value = nb.Value.ToString();
                        UpdateContent(); 
                    };
                }
                if(p.Value.ValueType == typeof(DifficultyEnum))
                {
                    ComboBox cb = new ComboBox();
                    cb.Width = 100;
                    cb.Margin = new Thickness(0, 0, 10, 0);
                    foreach(var de in Enum.GetValues(typeof(DifficultyEnum)))
                    {
                        string n = Enum.GetName(typeof(DifficultyEnum), de);
                        cb.Items.Add(n);
                    }
                    ce.Tag = "enum";
                    ce.HeaderContent = cb;
                    cb.SelectionChanged += (o, ee) =>
                    {
                        p.Value.Value = cb.SelectedValue.ToString();
                        UpdateContent();
                    };
                }
                if (p.Value.ValueType == typeof(GamemodeEnum))
                {
                    ComboBox cb = new ComboBox();
                    cb.Width = 100;
                    cb.Margin = new Thickness(0, 0, 10, 0);
                    foreach (var de in Enum.GetValues(typeof(GamemodeEnum)))
                    {
                        string n = Enum.GetName(typeof(GamemodeEnum), de);
                        cb.Items.Add(n);
                    }
                    ce.Tag = "enum";
                    ce.HeaderContent = cb;
                    cb.SelectionChanged += (o, ee) =>
                    {
                        p.Value.Value = cb.SelectedValue.ToString();
                        UpdateContent();
                    };
                }
                if (p.Value.ValueType == typeof(GenerateWorldTypeEnum))
                {
                    ComboBox cb = new ComboBox();
                    cb.Width = 100;
                    cb.Margin = new Thickness(0, 0, 10, 0);
                    foreach (var de in Enum.GetValues(typeof(GenerateWorldTypeEnum)))
                    {
                        string n = Enum.GetName(typeof(GenerateWorldTypeEnum), de);
                        cb.Items.Add(n);
                    }
                    ce.Tag = "enum";
                    ce.HeaderContent = cb;
                    cb.SelectionChanged += (o, ee) =>
                    {
                        p.Value.Value = cb.SelectedValue.ToString();
                        UpdateContent();
                    };
                }
                TextBlock tbb = new TextBlock();
                ce.Content = tbb;
                PropsBox.Children.Add(ce);
            }
            UpdatedValue();
        }

        private void OnModAddBtnClick(object sender, RoutedEventArgs e)
        {
            List<string> s = GetFileFromDialog(false, ".jar");
            foreach (string p in s)
            {
                CardControl c = new CardControl();
                WPFUI.Controls.Button b = new WPFUI.Controls.Button();
                b.Content = "削除";
                b.Appearance = WPFUI.Common.Appearance.Secondary;
                b.Click += (o, e) =>
                {
                    var t = (WPFUI.Controls.Button)o;
                    var cc = (CardControl)t.Parent;
                    _ModPath.Remove(cc.Subtitle);
                    ModsBox.Children.Remove(cc);
                    ModExpander.Subtitle = ModsBox.Children.Count + "個のモッド";
                };
                c.Title = System.IO.Path.GetFileName(p);
                c.Subtitle = p;
                c.Content = b;
                c.Margin = new Thickness(0, 0, 0, 10);
                ModsBox.Children.Add(c);
                ModExpander.Subtitle = ModsBox.Children.Count + "個のモッド";
                _ModPath.Add(p);
            }
        }

        private void OnPluginAddBtnClick(object sender, RoutedEventArgs e)
        {
            List<string> s = GetFileFromDialog(false, ".jar");
            if (s == null) return;
            foreach(string p in s)
            {
                CardControl c = new CardControl();
                WPFUI.Controls.Button b = new WPFUI.Controls.Button();
                b.Content = "削除";
                b.Appearance = WPFUI.Common.Appearance.Secondary;
                b.Click += (o, e) =>
                {
                    var t = (WPFUI.Controls.Button)o;
                    var cc = (CardControl)t.Parent;
                    _PluginPath.Remove(cc.Subtitle);
                    PluginsBox.Children.Remove(cc);
                    PluginExpander.Subtitle = PluginsBox.Children.Count + "個のプラグイン";
                };
                c.Title = System.IO.Path.GetFileName(p);
                c.Subtitle = p;
                c.Content = b;
                c.Margin = new Thickness(0, 0, 0, 10);
                PluginsBox.Children.Add(c);
                PluginExpander.Subtitle = PluginsBox.Children.Count + "個のプラグイン";
                _PluginPath.Add(p);
            }
        }

        private void OnWorldDataBtnClick(object sender, RoutedEventArgs e)
        {
            List<string> s = GetFileFromDialog(true, "", false);
            if (s == null) return;
            if (!ZipFormatCheckService.CheckWorldDataFormat(s[0]))
            {
                ((MainWindow)Application.Current.MainWindow).PopupBox.Title
                    = "ファイル形式が違います！";
                ((MainWindow)Application.Current.MainWindow).PopupBox.Icon
                    = WPFUI.Common.Icon.Warning24;
                ((MainWindow)Application.Current.MainWindow).PopupBox.Message
                    = "もしかして: フォルダがもう一階層あるかも？？？(フォルダ内にlevel.datがあればワールドデータです)";
                ((MainWindow)Application.Current.MainWindow).PopupBox.Expand();
                return;
            }
            _WorldDataPath = s[0];
            this.WorldDataBox.Subtitle = s[0];
            PropertiesService.Properties["level-name"].Value 
                = Path.GetFileName(_WorldDataPath);
            UpdatedValue();
        }

        private List<string> GetFileFromDialog(bool isFolder, string ext, bool isMultiSel = true)
        {
            using (CommonOpenFileDialog ofd = new CommonOpenFileDialog())
            {
                ofd.Multiselect = isMultiSel;
                ofd.IsFolderPicker = isFolder;
                if(!isFolder)
                    ofd.Filters.Add(new CommonFileDialogFilter(ext, ext));
                if(ofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    return ofd.FileNames.ToList();
                }
                return null;
            }
        }
    }
}
