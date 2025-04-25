using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordCloudGenerator : MonoBehaviour
{
    public RectTransform canvasTransform;
    public TextMeshProUGUI textPrefab;
    public Vector2 cloudSize = new Vector2(800, 600);
    public int maxFontSize = 60;
    public int minFontSize = 20;
    public bool generate = false;

    private List<Rect> placedRects = new List<Rect>();

    public Dictionary<string, int> wordFrequencies = new Dictionary<string, int>()
    {
        {"123456",100000},
        {"password",95000},
        {"12345678",90250},
        {"qwerty",85738},
        {"123456789",81451},
        {"12345",77378},
        {"1234",73509},
        {"111111",69834},
        {"1234567",66342},
        {"dragon",63025},
        {"123123",59874},
        {"baseball",56880},
        {"abc123",54036},
        {"football",51334},
        {"monkey",48767},
        {"letmein",46329},
        {"696969",44013},
        {"shadow",41812},
        {"master",39721},
        {"666666",37735},
        {"qwertyuiop",35849},
        {"123321",34056},
        {"mustang",32353},
        {"1234567890",30736},
        {"michael",29199},
        {"654321",27739},
        {"superman",25034},
        {"1qaz2wsx",23783},
        {"7777777",22594},
        {"121212",20391},
        {"000000",19371},
        {"qazwsx",18403},
        {"123qwe",17482},
        {"trustno1",15778},
        {"jordan",14989},
        {"jennifer",14240},
        {"zxcvbnm",13528},
        {"asdfgh",12851},
        {"hunter",12209},
        {"buster",11598},
        {"soccer",11018},
        {"harley",10467},
        {"batman",9944},
        {"andrew",9447},
        {"tigger",8974},
        {"sunshine",8526},
        {"iloveyou",8099},
        {"2000",7310},
        {"charlie",6944},
        {"robert",6597},
        {"thomas",6267},
        {"hockey",5954},
        {"ranger",5656},
        {"daniel",5373},
        {"starwars",5105},
        {"klaster",4849},
        {"112233",4607},
        {"george",4377},
        {"computer",3950},
        {"michelle",3752},
        {"jessica",3565},
        {"pepper",3387},
        {"1111",3217},
        {"zxcvbn",3056},
        {"555555",2904},
        {"11111111",2758},
        {"131313",2620},
        {"freedom",2489},
        {"777777",2365},
        {"pass",2247},
        {"maggie",2028},
        {"159753",1926},
        {"aaaaaa",1830},
        {"ginger",1738},
        {"princess",1652},
        {"joshua",1569},
        {"cheese",1491},
        {"amanda",1416},
        {"summer",1345},
        {"love",1278},
        {"ashley",1214},
        {"6969",1153},
        {"nicole",1096},
        {"chelsea",1041},
        {"matthew",939},
        {"access",892},
        {"yankees",848},
        {"987654321",805},
        {"dallas",765},
        {"austin",727},
        {"thunder",691},
        {"taylor",656},
        {"matrix",623},
        {"beginning-west-event-winter-flag",50},
        {"unknown-rush-arrow-meant-few",50},
        {"if-adventure-spirit-other-forget",50},
        {"sea-cookies-start-call-balloon",50},
        {"improve-club-habit-average-furniture",50},
        {"whom-cover-additional-broken-series",50},
        {"total-chest-wooden-program-victory",50},
        {"equator-pressure-action-blood-chain",50},
        {"religious-progress-whose-later-those",50},
        {"sure-brief-position-unknown-piano",50},
        {"pour-thought-cave-nearest-how",50},
        {"die-dropped-nature-yet-stairs",50},
        {"police-company-hold-beginning-highway",50},
        {"replace-discuss-herself-account-rhythm",50},
        {"structure-inside-white-send-weak",50},
        {"well-line-friendly-talk-magnet",50},
        {"silent-cage-act-path-shore",50},
        {"round-partly-diameter-up-touch",50},
        {"chart-fur-slope-remove-industry",50},
        {"only-receive-lot-accident-wrong",50},
        {"truck.same.way.forth.steep",50},
        {"purple.play.sea.depth.drawn",50},
        {"shake.eaten.let.town.stranger",50},
        {"amount.any.laugh.climate.bottle",50},
        {"hill.difficulty.instant.throw.it",50},
        {"police_snake_orange_rubbed_count",50},
        {"widely_hair_forth_victory_difference",50},
        {"arm_swept_available_smooth_solar",50},
        {"power_right_threw_canal_jar",50},
        {"said_who_neck_promised_my",50},
        {"movie_dawn_track_bean_our",50},
        {"trip_service_known_uncle_vegetable",50},
        {"shape-pitch-travel-trail-dangerous",50},
        {"Ants-Neighborhood-Am-Struggle-Stop",50},
        {"Them-Let-One-Tall-Held",50},
        {"Ate-Visit-Present-Composition-Introduced",50},
        {"further-mile-two-told-mass",50},
        {"band-mean-income-hurt-instrument",50},
        {"men-bet-stranger-arrangement-stronger",50},
        {"band-rock-middle-brain-hay",50},
        {"34298735482858787752",100},
        {"84233327226733466266",100},
        {"96856432835446888487",100},
        {"47742838234376958458",100},
        {"85994887759989823352",100},
        {"58628878873822868946",100},
        {"45698832879942267954",100},
        {"92338273654287688626",100},
        {"94982326844728342383",100},
        {"77947724533286974424",100},
        {"65298832358685488243",100},
        {"27694239649253529473",100},
        {"97346895789896562483",100},
        {"24525777888836323872",100},
        {"43645899252785877646",100},
        {"74588748975765379533",100},
        {"96894589859776757456",100},
        {"42689626428469793289",100},
        {"66883972436499962667",100},
        {"26928389425432657924",100},
        {"gKthKhdZGMKWSq",100},
        {"PSmCRTQSHdKfcR",100},
        {"dUFPbejUytRWxY",100},
        {"FjmwRSqrNzdcUn",100},
        {"CVKMggZdypZqLu",100},
        {"qsDQjzMrUQXRVw",100},
        {"SvkhWuSGyBtesq",100},
        {"vguJEmqURLCTpg",100},
        {"dAPYJynhkNcUWx",100},
        {"YNEkKVWJYACeQR",100},
        {"wKjfSwVthAGYua",100},
        {"wpNfNbpDHvFuXD",100},
        {"ZynPywMrkeCEeY",100},
        {"zMxeRvwejMTZYp",100},
        {"hUQtMESCTCBGcd",100},
        {"WAQeYQkwtGgtzu",100},
        {"EhSeTdTmXvvFDz",100},
        {"BaSphzTmLExumY",100},
        {"TqsfJKVDneLABu",100},
        {"pHsEcHTVYdryyx",100},
        {"HQ4z7pL3d22/&",100},
        {"GN*n+Sj+pc3U=",100},
        {"dJ3>w&Xy>cc9p",100},
        {"r=Zu-e}*Cz3Xs",100},
        {"HGs>2-*wLPaU)",100},
        {"9GLCnWhme4w/L",100},
        {"z6Tb*^Udc%3}[",100},
        {"8BfWq$J%B.mf-",100},
        {"Ywf@gX47XUr^h",100},
        {"K?J92e).n5Y?W",100},
        {"%CnF%95AWce.P",100},
        {"x/2hhSXkVefsE",100},
        {"T>5fPD$DJDpnX",100},
        {"$T3t>7D>XeK>V",100},
        {"49.!se7pS.}aN",100},
        {"(KU_C2^cGJ]z)",100},
        {"M}rgT5TwumfD?",100},
        {"pHqLqg$xK-Xe2",100},
        {"g{Aa3X*PVzan-",100},
        {"6?}d[P7V6=u!?",100},
    };

    void Update()
    {
        if (generate)
        {
            generate = false;
            GenerateWordCloud();
        }
    }

    void GenerateWordCloud()
    {
        if (wordFrequencies.Count == 0)
            return;

        int maxFreq = 1;
        foreach (var frequency in wordFrequencies.Values)
            if (frequency > maxFreq) maxFreq = frequency;

        foreach (var kvp in wordFrequencies)
        {
            var textInstance = Instantiate(textPrefab, canvasTransform);
            int fontSize = (int)Mathf.Lerp(minFontSize, maxFontSize, (float)kvp.Value / maxFreq);
            textInstance.fontSize = fontSize;
            textInstance.text = kvp.Key;
            textInstance.rectTransform.sizeDelta = new Vector2(textInstance.preferredWidth, fontSize);
            Vector2 pos = FindNonOverlappingPosition(textInstance.rectTransform.sizeDelta);
            textInstance.rectTransform.anchoredPosition = pos;
        }
    }

    Vector2 FindNonOverlappingPosition(Vector2 size)
    {
        for (int attempt = 0; attempt < 10000; attempt++)
        {
            Vector2 position = new Vector2(Random.Range(-cloudSize.x / 2, cloudSize.x / 2), Random.Range(-cloudSize.y / 2, cloudSize.y / 2));
            Rect newRect = new Rect(position - size / 2, size);
            bool overlaps = false;
            foreach (var r in placedRects)
            {
                if (r.Overlaps(newRect))
                {
                    overlaps = true;
                    break;
                }
            }
            if (!overlaps)
            {
                placedRects.Add(newRect);
                return position;
            }
        }
        return Vector2.zero;
    }
}
