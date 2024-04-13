using System;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Mediation;
using UnityEngine;

public class InterstitialAd : MonoBehaviour, IDisposable
{
    IInterstitialAd ad;
    string adUnitId = "Interstitial_Android";
    string gameId = "4983749";

    GameSystem m_gameSystem;

    void Awake ()
    {
        m_gameSystem = GameSystem.Instance;
    }

    public async Task InitServices()
    {
        try
        {
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetGameId(gameId);
            await UnityServices.InitializeAsync(initializationOptions);

            InitializationComplete();
        }
        catch (Exception e)
        {
            InitializationFailed(e);
        }
    }

    public void SetupAd()
    {
        //Create
        ad = MediationService.Instance.CreateInterstitialAd(adUnitId);

        //Subscribe to events
        ad.OnClosed += AdClosed;
        ad.OnClicked += AdClicked;
        ad.OnLoaded += AdLoaded;
        ad.OnFailedLoad += AdFailedLoad;
        
        // Impression Event
        MediationService.Instance.ImpressionEventPublisher.OnImpression += ImpressionEvent;
    }

    public void Dispose() => ad?.Dispose();

    
    public async void ShowAd()
    {
        if (ad.AdState == AdState.Loaded)
        {
            try
            {
                InterstitialAdShowOptions showOptions = new InterstitialAdShowOptions();
                showOptions.AutoReload = true;
                await ad.ShowAsync(showOptions);
                AdShown();
            }
            catch (ShowFailedException e)
            {
                AdFailedShow(e);
            }
        }
        else
        {
            m_gameSystem.NextLevel();
        }
    }

    void InitializationComplete()
    {
        SetupAd();
        Task task = LoadAd();
    }

    async Task LoadAd()
    {
        try
        {
            await ad.LoadAsync();
        }
        catch (LoadFailedException)
        {
            // We will handle the failure in the OnFailedLoad callback
        }
    }

    void InitializationFailed(Exception e)
    {

    }

    void AdLoaded(object sender, EventArgs e)
    {

    }

    void AdFailedLoad(object sender, LoadErrorEventArgs e)
    {
        m_gameSystem.NextLevel();
    }
    
    void AdShown()
    {

    }
    
    void AdClosed(object sender, EventArgs e)
    {
        m_gameSystem.NextLevel();
    }

    void AdClicked(object sender, EventArgs e)
    {

    }
    
    void AdFailedShow(ShowFailedException e)
    {
        m_gameSystem.NextLevel();
    }

    void ImpressionEvent(object sender, ImpressionEventArgs args)
    {
        var impressionData = args.ImpressionData != null ? JsonUtility.ToJson(args.ImpressionData, true) : "null";
        Debug.Log("Impression event from ad unit id " + args.AdUnitId + " " + impressionData);
    }
}