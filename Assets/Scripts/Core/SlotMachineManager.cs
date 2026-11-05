
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SlotMachineManager : MonoBehaviour
{
    [Header("Reels")]
    [SerializeField] private SlotReel[] reels;

    [Header("Coin Settings")]
    [SerializeField] private int startingCoins = 12500;
    [SerializeField] private int currentCoins;
    [SerializeField] private int currentBet = 100;
    [SerializeField] private int betStep = 100;

    [Header("Payout Multipliers")]
    [SerializeField] private float twoMatchMultiplier = 1.5f;
    [SerializeField] private float threeMatchMultiplier = 2f;

    [Header("UI References")]
    [SerializeField] private TMP_Text balanceText;
    [SerializeField] private TMP_Text betText;
    [SerializeField] private TMP_Text resultText;

    [Header("Buttons")]
    [SerializeField] private Button spinButton;
    [SerializeField] private Button betPlusButton;
    [SerializeField] private Button betMinusButton;
    [SerializeField] private Button maxBetButton;

    private bool isSpinning = false;

    private void Start()
    {
        currentCoins = startingCoins;

        spinButton.onClick.AddListener(SpinSlots);
        betPlusButton.onClick.AddListener(IncreaseBet);
        betMinusButton.onClick.AddListener(DecreaseBet);
        maxBetButton.onClick.AddListener(SetMaxBet);

        UpdateUI();
    }

    private void UpdateUI()
    {
        balanceText.text = currentCoins.ToString("N0");
        betText.text = currentBet.ToString("N0");

        // Disable betting when spinning or when coins are insufficient.
        betPlusButton.interactable = !isSpinning && currentBet + betStep <= currentCoins;
        betMinusButton.interactable = !isSpinning && currentBet > betStep;
        maxBetButton.interactable = !isSpinning && currentCoins >= 100;
        spinButton.interactable = !isSpinning && currentCoins >= currentBet;
    }

    private void IncreaseBet()
    {
        if (isSpinning || currentBet + betStep > currentCoins)
            return;

        currentBet += betStep;
        UpdateUI();
    }

    private void DecreaseBet()
    {
        if (isSpinning || currentBet <= betStep)
            return;

        currentBet -= betStep;
        UpdateUI();
    }

    private void SetMaxBet()
    {
        if (isSpinning)
            return;

        currentBet = (currentCoins / betStep) * betStep;
        UpdateUI();
    }

    private void SpinSlots()
    {
        if (isSpinning || currentCoins < currentBet)
            return;

        StartCoroutine(SpinAllReels());
    }

    private IEnumerator SpinAllReels()
    {
        isSpinning = true;

        // Deduct the bet before spinning.
        currentCoins -= currentBet;

        resultText.text = "Spinning...";
        spinButton.interactable = false;

        UpdateUI();

        // Stop reels one after another.
        yield return StartCoroutine(reels[0].Spin(2f));
        yield return StartCoroutine(reels[1].Spin(2.5f));
        yield return StartCoroutine(reels[2].Spin(3f));

        CheckWinning();

        isSpinning = false;

        UpdateUI();

        spinButton.interactable = currentCoins >= currentBet;
    }

    private void CheckWinning()
    {
        Sprite first = reels[0].CurrentSymbol;
        Sprite second = reels[1].CurrentSymbol;
        Sprite third = reels[2].CurrentSymbol;

        if (first == second && second == third)
        {
            int payout = Mathf.RoundToInt(currentBet * threeMatchMultiplier);

            currentCoins += payout;

            resultText.text = "JACKPOT! You won " + payout.ToString("N0") + " coins!";
        }
        else if (first == second || second == third || first == third)
        {
            int payout = Mathf.RoundToInt(currentBet * twoMatchMultiplier);

            currentCoins += payout;

            resultText.text = "NICE! You won " + payout.ToString("N0") + " coins!";
        }
        else
        {
            resultText.text = "TRY AGAIN! No reward.";
        }
    }
}
