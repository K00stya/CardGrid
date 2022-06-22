using UnityEngine.Events;

namespace CardGrid
{
    /*
     * Interface through which interaction with monetization plugins takes place
     */
    public class MonetizeSystem
    {
        public static void TryShowAds(out UnityEvent<bool> adsViewSuccess)
        {
            adsViewSuccess = new UnityEvent<bool>();

            adsViewSuccess.Invoke(true);
        }

        public static void TryBuyItem(string name, out UnityEvent<bool> buySuccess)
        {
            buySuccess = new UnityEvent<bool>();

            buySuccess.Invoke(true);
        }
    }
}