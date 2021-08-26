using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OStimAnimationTool.Core.Models.Navigation;

namespace AnimationDatabaseExplorer
{
    public class OptionIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource source = value switch
            {
                OptionIcons.o1fpufrx_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/o1fpufrx_mf.png")),
                OptionIcons.o1fpufr_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/o1fpufr_mf.png")),
                OptionIcons.o2fpufrx_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/o2fpufrx_mf.png")),
                OptionIcons.o2fpufr_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/o2fpufr_mf.png")),
                OptionIcons.obalhos2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obalhos2_f.png")),
                OptionIcons.obb1freartx_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obb1freartx_mf.png")),
                OptionIcons.obb1freart_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obb1freart_mf.png")),
                OptionIcons.obb2freartx_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obb2freartx_mf.png")),
                OptionIcons.obb2freart_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obb2freart_mf.png")),
                OptionIcons.obbpenrearts2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbpenrearts2_f.png")),
                OptionIcons.obbpenrearts2_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbpenrearts2_m.png")),
                OptionIcons.obbpenxrearts2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbpenxrearts2_f.png")),
                OptionIcons.obbpenxrearts2_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbpenxrearts2_m.png")),
                OptionIcons.obbspankl_6_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbspankl_6_f.png")),
                OptionIcons.obbspankr_6_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbspankr_6_f.png")),
                OptionIcons.obbstup_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obbstup_f.png")),
                OptionIcons.obigx => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obigx.png")),
                OptionIcons.objbf_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/objbf_f.png")),
                OptionIcons.objbos_mf => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/objbos_mf.png")),
                OptionIcons.obododn_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obododn_f.png")),
                OptionIcons.obodoup_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obodoup_f.png")),
                OptionIcons.obombpenrears2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obombpenrears2_f.png")),
                OptionIcons.obustt180_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/obustt180_f.png")),
                OptionIcons.ocurlidn_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ocurlidn_9_f.png")),
                OptionIcons.ocurliup_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ocurliup_9_f.png")),
                OptionIcons.ofdaut_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ofdaut_f.png")),
                OptionIcons.ohandsoffbody_ff => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohandsoffbody_ff.png")),
                OptionIcons.ohandsoffbody_fm => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohandsoffbody_fm.png")),
                OptionIcons.ohandsoffbody_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohandsoffbody_mf.png")),
                OptionIcons.ohandsoffbody_mm => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohandsoffbody_mm.png")),
                OptionIcons.ohgpenfrs2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohgpenfrs2_f.png")),
                OptionIcons.ohgpenxfrs2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohgpenxfrs2_f.png")),
                OptionIcons.ohjdubstand_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjdubstand_mf.png")),
                OptionIcons.ohjs2_9_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjs2_9_f.png")),
                OptionIcons.ohjs2_9_m => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjs2_9_m.png")),
                OptionIcons.ohjugs2_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjugs2_9_f.png")),
                OptionIcons.ohjugs2_9_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjugs2_9_m.png")),
                OptionIcons.ohjugxs2_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjugxs2_9_f.png")),
                OptionIcons.ohjugxs2_9_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjugxs2_9_m.png")),
                OptionIcons.ohjxs2_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjxs2_9_f.png")),
                OptionIcons.ohjxs2_9_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ohjxs2_9_m.png")),
                OptionIcons.oholdbody_8_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oholdbody_8_mf.png")),
                OptionIcons.oikea_mf => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oikea_mf.png")),
                OptionIcons.okalbdn_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/okalbdn_f.png")),
                OptionIcons.okalbup_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/okalbup_f.png")),
                OptionIcons.oknlbdn_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oknlbdn_f.png")),
                OptionIcons.oknlbup_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oknlbup_f.png")),
                OptionIcons.olipsx => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/olipsx.png")),
                OptionIcons.oludn_6_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oludn_6_f.png")),
                OptionIcons.oludn_9_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oludn_9_f.png")),
                OptionIcons.oluup_6_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluup_6_f.png")),
                OptionIcons.oluup_9_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluup_9_f.png")),
                OptionIcons.oluxdn_6_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluxdn_6_f.png")),
                OptionIcons.oluxdn_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluxdn_9_f.png")),
                OptionIcons.oluxup_6_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluxup_6_f.png")),
                OptionIcons.oluxup_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oluxup_9_f.png")),
                OptionIcons.omgsholds2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/omgsholds2_f.png")),
                OptionIcons.omgsholds2_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/omgsholds2_m.png")),
                OptionIcons.omgsletgos2_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/omgsletgos2_f.png")),
                OptionIcons.omgsletgos2_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/omgsletgos2_m.png")),
                OptionIcons.oplumpf180_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oplumpf180_f.png")),
                OptionIcons.oplumppenx_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oplumppenx_f.png")),
                OptionIcons.oplumppen_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oplumppen_f.png")),
                OptionIcons.opowemb_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/opowemb_mf.png")),
                OptionIcons.opowkiss_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/opowkiss_mf.png")),
                OptionIcons.osbtkstra_6_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osbtkstra_6_f.png")),
                OptionIcons.osc_bjportrait_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_bjportrait_mf.png")),
                OptionIcons.osc_cowgirl_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_cowgirl_mf.png")),
                OptionIcons.osc_handlebj_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_handlebj_f.png")),
                OptionIcons.osc_mufold_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_mufold_mf.png")),
                OptionIcons.osc_pantypeelrear_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_pantypeelrear_f.png")),
                OptionIcons.osc_powerkiss_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_powerkiss_mf.png")),
                OptionIcons.osc_sexcradle_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_sexcradle_mf.png")),
                OptionIcons.osc_wizsex_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osc_wizsex_mf.png")),
                OptionIcons.oslnbobkd_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oslnbobkd_f.png")),
                OptionIcons.oslnbobku_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oslnbobku_f.png")),
                OptionIcons.oslrevayd_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oslrevayd_f.png")),
                OptionIcons.oslrevayu_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/oslrevayu_f.png")),
                OptionIcons.osn_lflotus_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osn_lflotus_mf.png")),
                OptionIcons.osn_st6ho_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osn_st6ho_mf.png")),
                OptionIcons.osn_st9ho_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osn_st9ho_mf.png")),
                OptionIcons.osn_stknap_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osn_stknap_mf.png")),
                OptionIcons.osqtlbbdn_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osqtlbbdn_f.png")),
                OptionIcons.ostbkstra_9_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ostbkstra_9_f.png")),
                OptionIcons.ostgbo45_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/ostgbo45_f.png")),
                OptionIcons.osx_180bust_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_180bust_f.png")),
                OptionIcons.osx_bklomau_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_bklomau_f.png")),
                OptionIcons.osx_bodd_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_bodd_f.png")),
                OptionIcons.osx_bodo_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_bodo_f.png")),
                OptionIcons.osx_bom_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_bom_f.png")),
                OptionIcons.osx_do_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_do_f.png")),
                OptionIcons.osx_embkiss_mf => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_embkiss_mf.png")),
                OptionIcons.osx_feettoesup_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_feettoesup_f.png")),
                OptionIcons.osx_jerk_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_jerk_m.png")),
                OptionIcons.osx_ka_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_ka_f.png")),
                OptionIcons.osx_kf_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_kf_f.png")),
                OptionIcons.osx_kn6x_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_kn6x_f.png")),
                OptionIcons.osx_kn6y_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_kn6y_f.png")),
                OptionIcons.osx_kn_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_kn_f.png")),
                OptionIcons.osx_kn_m => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_kn_m.png")),
                OptionIcons.osx_layonback_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_layonback_f.png")),
                OptionIcons.osx_layside_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_layside_f.png")),
                OptionIcons.osx_legcurl_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_legcurl_f.png")),
                OptionIcons.osx_letgomale_m => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_letgomale_m.png")),
                OptionIcons.osx_mu01_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_mu01_f.png")),
                OptionIcons.osx_mu02_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_mu02_f.png")),
                OptionIcons.osx_mu03_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_mu03_f.png")),
                OptionIcons.osx_penpusrearout_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_penpusrearout_f.png")),
                OptionIcons.osx_penpusrear_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_penpusrear_f.png")),
                OptionIcons.osx_spankl_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_spankl_f.png")),
                OptionIcons.osx_spankr_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_spankr_f.png")),
                OptionIcons.osx_sqt_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_sqt_f.png")),
                OptionIcons.osx_starch_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_starch_f.png")),
                OptionIcons.osx_stbkstra_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_stbkstra_f.png")),
                OptionIcons.osx_stlegspr_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_stlegspr_f.png")),
                OptionIcons.osx_st_f => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_st_f.png")),
                OptionIcons.osx_st_m => new BitmapImage(
                    new Uri("pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_st_m.png")),
                OptionIcons.osx_thispr_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_thispr_f.png")),
                OptionIcons.osx_tumli_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/osx_tumli_f.png")),
                OptionIcons.otiptoedn_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/otiptoedn_f.png")),
                OptionIcons.otiptoeup_f => new BitmapImage(new Uri(
                    "pack://application:,,,/AnimationDatabaseExplorer;component/Icons/Option/otiptoeup_f.png")),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };

            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
