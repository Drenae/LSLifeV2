namespace LSlife;

internal static partial class LsMenus
{
    internal static void CallWeaponDealer() => ZeeContactActions.CallWeaponDealer();
    internal static void CallLift() => ZeeContactActions.CallLift();
    internal static void PayZeeDebt() => ZeeContactActions.PayZeeDebt();
    internal static void CancelDealerRecruitment() => DealerRecruitmentActions.CancelDealerRecruitment();
    internal static void SubmitDealerOffer(int amount) => DealerRecruitmentActions.SubmitDealerOffer(amount);
}
