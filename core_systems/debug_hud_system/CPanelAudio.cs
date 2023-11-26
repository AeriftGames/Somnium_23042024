using Godot;
using System;

public partial class CPanelAudio : CPanelBase
{
    public override void PostInit(CDebugPanel newDebugPanel)
    {
        base.PostInit(newDebugPanel);
    }

    public override void LoadAllElementsSettings()
    {
        // main volume
        HSlider mainVolumeSlider = GetNode<HSlider>("%mainVolume_HSlider");
        mainVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_MainVolume();

        Label mainVolumeLabel = GetNode<Label>("%mainVolume_Label");
        mainVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_MainVolume().ToString() + " db";

        // sfx volume
        HSlider sfxVolumeSlider = GetNode<HSlider>("%sfxVolume_HSlider");
        sfxVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_SfxVolume();

        Label sfxVolumeLabel = GetNode<Label>("%sfxVolume_Label");
        sfxVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_SfxVolume().ToString() + " db"; ;

        // sfx volume
        HSlider musicVolumeSlider = GetNode<HSlider>("%musicVolume_HSlider");
        musicVolumeSlider.Value = CGameMaster.GM.GetSettings().GetActual_MusicVolume();

        Label musicVolumeLabel = GetNode<Label>("%musicVolume_Label");
        musicVolumeLabel.Text = CGameMaster.GM.GetSettings().GetActual_MusicVolume().ToString() + " db"; ;
    }

    public override void SaveAllElementsSettings()
    {
        base.SaveAllElementsSettings();

        CGameMaster.GM.GetSettings().SaveActual_AllAudioSettings();
    }

    public void _on_main_volume_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_MainVolume(newValue, true, false);

        // update label
        Label mainVolume_label = GetNode<Label>("%mainVolume_Label");
        mainVolume_label.Text = newValue.ToString() + " db"; ;
    }
    public void _on_sfx_volume_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_SfxVolume(newValue, true, false);

        // update label
        Label sfxVolume_label = GetNode<Label>("%sfxVolume_Label");
        sfxVolume_label.Text = newValue.ToString() + " db"; ;
    }
    public void _on_music_volume_h_slider_value_changed(float newValue)
    {
        // only apply
        CGameMaster.GM.GetSettings().Apply_MusicVolume(newValue, true, false);

        // update label
        Label musicVolume_label = GetNode<Label>("%musicVolume_Label");
        musicVolume_label.Text = newValue.ToString() + " db"; ;
    }
}
