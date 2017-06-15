using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ZXing.Mobile;
using conseilMoi.Classes;

namespace conseilMoi
{
    [Activity(Label = "Conseil")]
    public class Conseil : Activity
    {
        ExpandableListViewAdapter_TexteRelatif mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();

        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Conseil);

            expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewConditionUtilisation);
            SetData(out mAdapter);
            expandableListView.SetAdapter(mAdapter);

            expandableListView.ChildClick += (s, e) =>
            {
                Toast.MakeText(this, "clicked: " + mAdapter.GetChild(e.GroupPosition, e.ChildPosition), ToastLength.Short).Show();

            };


            MobileBarcodeScanner.Initialize(Application);

            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewConseilProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewConseilHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewConseilScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewConseilConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewConseilAvertissement);


            menuProfil.Click += delegate
            {
                StartActivity(typeof(Profil));
            };
            menuHistorique.Click += delegate
            {
                StartActivity(typeof(Historique));
            };

            menuConseil.Click += delegate
            {
                StartActivity(typeof(Conseil));
            };

            //Clik sur le bouton scanner
            menuScanner.Click += async (sender, e) =>
            {
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();
                var result = await scanner.Scan();
                if (result != null)
                {
                    //Intent garde la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    produit.PutExtra("IDproduit", result.Text);
                    StartActivity(produit);
                }
                else { }
            };

            menuAvertissement.Click += delegate
            {
                StartActivity(typeof(Avertissement));
            };

        }
        private void SetData(out ExpandableListViewAdapter_TexteRelatif mAdapter)
        {
            List<string> groupA = new List<string>();
            groupA.Add(
            "Souvent associés aux problématiques de surpoids et d’obésité, les lipides(appelées aussi « graisses ») ont créés ces dernières années une véritable frénésie « anti - graisses ». Pourtant les lipides sont essentielles dans le fonctionnement de notre organisme. \n\n"
            + "Les lipides\n"
            + " Qu’est - ce que les lipides ?\n"
            + "Les lipides sont un des trois macronutriments (= nutriment qui fournit de l’énergie) indispensable au bon fonctionnement de notre organisme, aux côtés des glucides et des protéines. Dans le langage courant, on a souvent tendance à parler de « graisses » ."
            + "Les lipides sont donc des graisses; des graisses indispensables pour notre organisme (ex: notre cerveau est constitué d’au moins 50 % de lipides). Tout de même,  les lipides jouissent d’une image très négative liée au rôle qu’elles exercent dans l’apparition de pathologies telles que les maladies cardiovasculaires, l’obésité, les maladies neurovasculaires, etc. \n\n"
            + "Le rôle des lipides au sein de notre organisme\n"
           + " Les lipides jouent un rôle important dans notre organisme puisqu’ils permettent de nous fournir de l’énergie.Un gramme de graisses fournit 9 kilocalories, soit plus du double de ce que fournissent les protéines et les glucides."
+ "Les lipides sont stockées dans notre organisme et permettent de fournir de l’énergie lors d’un effort de longue durée, lors d’un jeûne prolongé ou encore entre deux repas."
+ "Les différents types de lipides"
+ "Il existe plusieurs groupes de lipides: nous allons uniquement évoquer ici les acides gras."
+ "Les acides gras"
+ "On distingue 3 types d’acides gras:"
+ "•	Les acides gras saturés: bien qu’ils permettent d’apporter de l’énergie à notre organisme, ils convient néanmoins de les consommer avec modération(pas plus de 25 % des apports lipidiques quotidiens).En effet, appelés également « mauvaises graisses », ils augmentent le mauvais cholestérol(LDL) ainsi que les risque de mortalité coronarienne.\n"
+ "•	Les acides gras mono - insaturés: contrairement aux acides gras saturés, ils ont un effet bénéfique sur le cholestérol.Ils diminuent également les risques de maladies cardiovasculaires et d’hypertension, ils devraient représentés 65 % des apports en lipides journaliers.La principale forme d’acide gras mono - insaturé est l’acide oléique(oméga 9).\n"
+ "•	Les acides gras polyinsaturés qui peuvent être fabriqués par l’organisme à partir de deux acides gras précurseurs(l’acide linoléique, oméga 6, et l’acide alpha - linolénique, pour les oméga 3).Ces acides gras précurseurs sont appelés acides gras essentiels(ou indispensables) car le corps ne sait pas les fabriquer, nous devons donc les apporter par l’alimentation.\n"
+ "•	On peut également citer les acides gras trans, qui sont des acides chimiquement transformés en laboratoire, créés par l’industrie agroalimentaire.Ils augmenteraient la peroxydation des lipides(= formation de radicaux libres responsables de vieillissement prématuré et de dommages cellulaires au niveau de l’organisme) et favorisent le dépôt de lipides sur les parois internes des vaisseaux.\n"

+ "Les acides gras essentiels"
+ "On peut également classer les acides gras d’un point de vue physiologique.Ainsi, on peut distinguer:"
+ "•	les acides gras indispensables nécessaires au développement et au bon fonctionnement du corps humain, mais que notre corps ne sait pas fabriquer ;\n"
+ "•	les acides gras conditionnellement indispensables, essentiels pour la croissance normale et les fonctions physiologiques des cellules mais qui peuvent être fabriqués à partir de leur précurseur s’il est apporté par l’alimentation.\n"
+ "•	Les acides gras non indispensables(« non essentiels »), que notre organisme sait synthétiser.\n"
+ "L’ensemble des acides gras indispensables et conditionnellement indispensables constituent les acides gras essentiels.On compte deux grandes familles d’acides gras essentiels:"
+ "•	les acides gras polyinsaturés oméga 6 dont le précurseur indispensable est l’acide linoléique.\n"
+ "•	les acides gras polyinsaturés oméga 3 dont le précurseur indispensable est l’acide alpha-linolénique.\n"

            );

            List<string> groupB = new List<string>();
            groupB.Add(
            "Les protides, communément appelés protéines, ne se trouvent pas uniquement dans la viande et ne sont pas seulement pour les sportifs!"
+ " Les Protides"
+ " Qu’est - ce qu’un protide ?"
+ " Un protide est l’un des trois macronutriments(= nutriment qui fournit de l’énergie) indispensables au bon fonctionnement de notre organisme, aux côtés des glucides et des lipides."
+ " Dans le langage courant, on a souvent tendance à parler de « protéines » par abus de langage.En réalité, il y a bien une différence entre les deux termes.Contrairement aux protéines, les protides sont, en quelque sorte, une « famille » regroupant les protéines, les acides aminés et les peptides – alors que les protéines sont des molécules faisant partie de la famille des protides."

+ " Le rôle des protides"
+ " Les protides sont essentiels dans le fonctionnement de l’être humain: ils sont présents dans toutes les cellules de notre organisme.Ils sont ainsi les constituants de:"
+ " •	Nos cheveux, nos ongles, notre peau(sous forme d’une protéine appelée « kératine »);\n"
+ " •	Nos tissus musculaires(sous forme de « myosine », « actine », « myoglobine »…);\n"
+ " •	Nos globules rouges(sous forme de « globine »).\n"
+ " Présents sous de multiples formes, ils assurent différentes fonctions au sein de notre organisme:"
+ " •	Ils forment et maintiennent la structure des composants de notre corps(ex: os, muscles, etc).\n"
+ " •	Ils participent au renouvellement cellulaire: Les acides aminés trouvés dans les protéines que nous mangeons sont absorbés puis utilisés par l’organisme pour remplacer les cellules endommagées ou non efficaces.\n"
+ " •	Grâce à leur contenance en azote, ils ont ce qu’on appelle un rôle compensatoire: ils compensent nos pertes en urines (azote rejeté par voie urinaire ou fécale) grâce à l’azote ingéré, contenu dans l’alimentation: dans les protéines en l’occurrence(nous en parlerons plus loin).\n"
+ " •	Les protides peuvent exercer des fonctions spécifiques telles que la protection de notre organisme vis - à - vis de l’extérieur(exemple: cheveux, ongles, peau…) ou encore la contraction des muscles (cas des protéines « contractiles » : l’actine et la myosine).\n"
+ " •	D’autres protéines assurent des fonctions physiologiques comme la digestion(cas des enzymes), la transmission d’informations(cas des neurotransmetteurs), la défense immunitaire(cas des immunoglobulines), etc.\n"
+ " •	Les protides ont également un rôle énergétique.En effet, elles permettent la création d’énergie.Ce rôle est néanmoins secondaire et apparaît uniquement en cas de jeûne prolongé: le corps dégrade alors les protéines pour fournir de l’énergie à notre organisme, néanmoins cela présente l’inconvénient d’entraîner des fontes musculaires, des baisses du tonus musculaire, voir des problèmes plus graves.\n"

+ " Apports nutritionnels conseillés(ANC) en protides"
+ " Au sein d’une alimentation adaptée, les protides devraient représenter 11 à 15 % de l’AET(apport énergétique total) journalier."
+ " Les apports nutritionnels conseillés varient selon le type de population: pour les adultes en bonne santé, L’ANSES conseille un apport de 0,83 g / kg / jour."

+ " * Données en g.kg - 1.j - 1 ==> grammes / kilos / jour(il convient donc de multiplier les données par le poids de l’individu)"
+ " Carences en protéines"
+ " Nous l’avons vu: les protides sont présentes dans toutes les cellules de notre organisme et ils exercent tout un tas de fonctions différentes. Pour cette raison, un apport insuffisant en protides aurait de lourdes conséquences sur notre organisme."
+ " Ne sachant pas synthétiser les acides aminés et ne disposant d’aucune réserve, notre organisme va puiser les protéines dans les muscles pour pouvoir fonctionner correctement. Or, cela entraîne une fonte musculaire importante – tout aussi importante pour les sportifs que pour le reste de la population."
+ " Risques d’excès"
+ " Une alimentation hyper-protéinée conduit à des risques de carences en minéraux et oligo-éléments.Elle peut également causer des complications rénales, car le surplus de protéines sollicite beaucoup nos reins. Cliquez ici pour en savoir plus sur les risques d’un régime trop riche en protéines."
+ " Il est difficile de définir une limite supérieure de sécurité pour l’apport protéique. Selon l’ANSES, des apports entre 0,83 et 2,2 g / kg / j de protéines (soit de 10 à 27 % de l’apport énergétique) peuvent être considérés comme satisfaisants pour un individu adulte moyen. Au delà, ils comporteraient des risques."
    + " Les principales sources de protides"
    + " Les protides peuvent provenir de deux sources différentes: les aliments d’origine animale et les aliments d’origine végétale :"
+ " – Les protéines animales sont fournies par les viandes rouges et blanches, les charcuteries, les poissons et les fruits de mer, les œufs, le lait, les fromages et les laitages (yaourt, fromage blanc).Elles sont plus riches en acides aminés indispensables que les protéines végétales et mieux digérées.Les aliments d’origine animale sont caractérisés par leur forte teneur en protéines de haute qualité nutritionnelle(composition en acides aminés indispensables, digestibilité, etc.)."
+ " – Les protéines végétales, elles, proviennent de deux grandes sources: les céréales(blé, avoine, riz, mais, etc) et les légumineuses(soja, lentilles, haricot, pois).Certaines protéines végétales peuvent présenter une teneur limitante en acides aminés indispensables, c’est pourquoi on recommande d’associer différents aliments végétaux pour apporter à notre organisme tous les acides aminés nécessaires et ainsi permettre le fonctionnement optimal de nos cellules.En effet, car l’absence ou l’insuffisance d’un acide aminé bloque ou limite la synthèse de nouvelles protéines, ce qui pourrait avoir des conséquences désastreuses sur le fonctionnement de notre organisme."
+ " Il est important néanmoins de varier l’origine des protéines.En effet, celle - ci peut avoir une incidence sur la couverture des besoins en d’autres nutriments.Ainsi:"
+ " •	une alimentation exclusivement d’origine végétale peut conduire à un risque de déficience en vitamine B12.\n"
+ " •	une alimentation riche en protéines animales peut conduire à un apport insuffisant en fibres et excessif en graisses saturées.\n"

                );

            List<string> groupD = new List<string>();
            groupD.Add(" Plébiscités par certains pour leur source d’énergie et incriminés par d’autres pour leurs rôles dans les problèmes de surpoids et d’obésité."
+ " Les glucides"
+ " Qu’est - ce que les glucides ?"
+ " Les glucides sont, aux côtés des lipides et des protéines, un des 3 nutriments énergétiques(= nutriments qui fournissent de l’énergie) indispensables au bon fonctionnement de notre organisme."
+ " Et ceux - là en particulier sont importants car ils ont la particularité d’apporter la plus large part de l’énergie de notre organisme: plus de la moitié de notre apport énergétique total."
+ " Les différents types de glucides"
+ " On en distingue trois types: les glucides simples, glucides complexes et les édulcorants."
+ " 1.Les glucides simples"
+ " Ils comprennent les sucres, divisés en deux groupes:"
+ " •	Les monosaccharides: qui englobent le gluctose, le fructose(provenant des fruits et légumes), le galactose ou encore le tagatose.\n"
+ " •	Les disaccharides: dont les plus connus sont le saccharose, le lactose ou encore le maltose.\n"
+ " Ils sont cariogènes(créateurs de caries) et sources de calories vides, c’est pourquoi ils devraient représentés moins de  10 % des apports énergétiques totaux."
+ " 2.Les glucides complexes"
+ " Ils comprennent les oligosaccharides et les polysaccharides à privilégier(produits céréaliers complets ou peu raffinés)."
+ " 3.Les polyols"
+ " Le rôle des glucides"
+ " Les glucides sont essentiels pour notre organisme, car ils constituent le principal carburant de notre corps.Ils permettent la contraction des muscles et permettent à notre cerveau de fonctionner correctement."
+ " Les risques d’un régime alimentaire pauvre en glucides"
+ " Un régime alimentaire pauvre en glucides est appelé « régime cétogène »."
+ " Il faut savoir qu’il peut engendrer:"
+ " – un état d’hypoglycémie(= chute du taux de sucre dans le sang) qui se traduit par des malaises: tremblement, éblouissement, troubles de l’humeur, troubles de l’élocution, accélération du pouls, perception des battements cardiaques, sudation, fringales, etc."
+ " – une fonte musculaire car l’organisme fabrique des glucides à partir des protéines du muscle."
+ " Conséquences d’un régime trop riche en glucides"
+ " Une consommation trop importante de glucides et notamment de sucres simples augmente l’apport calorique journalier.Si l’activité physique n’est pas elle aussi augmentée, l’organisme transforme les sucres en graisses et les stocke dans les cellules graisseuses.Cela favorise donc la prise de poids et peut conduire, à terme, à l’obésité."
                );


            group.Add("1. Les lipides");
            group.Add("2. Les protéines");
            group.Add("3. Les glucides");

            dicMyMap.Add(group[0], groupA);
            dicMyMap.Add(group[1], groupB);
            dicMyMap.Add(group[2], groupD);

            mAdapter = new ExpandableListViewAdapter_TexteRelatif(this, group, dicMyMap);
        }

        
    }
}