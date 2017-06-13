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
using conseilMoi.Resources.MaBase;
using conseilMoi.Resources.Classes;
using conseilMoi.Classes;
using System.Net;
using Android.Graphics;
using static System.Net.Mime.MediaTypeNames;
using System.Threading.Tasks;
using Android.Util;

namespace conseilMoi
{
    [Activity(Label = "Produit")]
    public class Produit : Activity
    {
        //ListView lstData;
        //List<ProduitRecos> lstSource = new List<ProduitRecos>();
        MaBase db1 = new MaBase();
        int feu = 0;
        
        string IDproduit;

        ExpandableListViewAdapter_DetailProduit mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();
        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();
        LinearLayout LinearNutriment;
        LinearLayout LinearAllergene; 
        MaBase db = new MaBase();
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Produit);

            //initialise la classe MaBase et connecte la base de donnnées
            db.ExistBase(this);
            db1.ExistBase(this);

            //Ajoute les 2 catégories allergene et nutriement pour l'affichage en détail
            group.Add("Détail allergènes");
            group.Add("Détail Nutriments");

            //initialise le scanner de code barre
            MobileBarcodeScanner.Initialize(Application);

            //Récupère le ID_Produit lorsque l'on scanne un produit
             IDproduit = Intent.GetStringExtra("IDproduit") ?? "Data not available";
            string IDTypeProfil = "PERS";




            //chargement des variables des boutons et textViews
            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewProduitProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewProduitHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewProduitScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewProduitConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewProduitAvertissement);
            var txtIdProduit = FindViewById<TextView>(Resource.Id.textViewIdProduit);
            var txtInfoScan = FindViewById<TextView>(Resource.Id.textViewInfoScan);
            var txtInfoAllergene = FindViewById<TextView>(Resource.Id.textViewInfoAlergene);
            var txtInfoNutriment = FindViewById<TextView>(Resource.Id.textViewInfoNutriment);
            var btnProduitPERS = FindViewById<Button>(Resource.Id.buttonProduitPERS);
            var btnProduitFAM = FindViewById<Button>(Resource.Id.buttonProduitFAM);
            var btnProduitINV = FindViewById<Button>(Resource.Id.buttonProduitINV);
            ImageView imgProduit = FindViewById<ImageView>(Resource.Id.imageViewProduit);
            ImageView imgFeu = FindViewById<ImageView>(Resource.Id.imageViewFeu);
            LinearAllergene = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMatchAllergene);
            LinearNutriment = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMatchNutriment);

            btnProduitINV.Text = "Mes amis";
            txtInfoAllergene.SetTextSize(ComplexUnitType.Dip, 18);
            txtInfoNutriment.SetTextSize(ComplexUnitType.Dip, 18);
            txtIdProduit.SetTextSize(ComplexUnitType.Dip, 20);
            txtInfoScan.SetTextSize(ComplexUnitType.Dip, 16);
            //var listNutriment = FindViewById<ExpandableListView>(Resource.Id.expandableListViewMatchAllNut);



            btnProduitPERS.SetBackgroundColor(Color.LightGray);
            btnProduitFAM.SetBackgroundColor(Color.Gray);
            btnProduitINV.SetBackgroundColor(Color.Gray);


            btnProduitPERS.SetTextColor(Color.Gray);
            btnProduitFAM.SetTextColor(Color.LightGray);
            btnProduitINV.SetTextColor(Color.LightGray);





            //On charge le produit Le IdProduit va dans le texteView IdProduit 
            //On créer d'abbord un objet produit qui contiendra tout le contenu du produit
            Produits produits = new Produits();
            produits = db.SelectIdProduit(IDproduit, IDTypeProfil);

        //SI PAS DE PRODUIT TROUVE
        if (produits.GetId_Produit() == "000") {
                imgProduit.SetImageResource(Resource.Drawable.produitnontrouve);
                txtIdProduit.Text = "Produit non trouvé";
                txtInfoScan.Text = "Code barre : " + IDproduit;
                txtInfoAllergene.Text = "Le produit n'existe ";
                txtInfoNutriment.Text = "pas dans la base";
                imgFeu.Visibility = ViewStates.Invisible;
            }
        //FIN SI PAS DE PRODUIT TROUVE

        //DEBUTE ELSE POUR SI PRODUIT TROUVE
        else { 

                imgFeu.Visibility = ViewStates.Visible;

                //Fait un enregistrement dans historique
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);

                imgProduit.SetImageResource(Resource.Drawable.pasimage);

                // CHARGEMENT DE L'IMAGE
                String ImageURL = produits.GetUrl();
             if (ImageURL != "") { 
                    Koush.UrlImageViewHelper.SetUrlDrawable(imgProduit, produits.GetUrl());
                }

             if (produits.GetProduct_name() == "") { txtIdProduit.Text = "Produit incomplet"; imgFeu.Visibility = ViewStates.Invisible; }
             else { txtIdProduit.Text = produits.GetProduct_name(); }
                
                txtInfoScan.Text = "Code barre : " + IDproduit;

            /* VERIFICATION POUR LE CHARGEMENT PAR DEFAUT SUR LE PROFIL PERSO */
            /* VERIFIE LES ALLERGENES */
            List<Allergene> ListAl = new List<Allergene>();
            ListAl = produits.GetCheckAllergene();


            try
            {//Si aucun allergene dans la liste, alors le produit est comptatible avec le produit
                if (ListAl[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "Allergenes compatibles";  }
                //Sinon la simple présence d'au moins 1 allergene dans la liste, rends le produit incompatible
                else { txtInfoAllergene.Text = "Allergenes incompatible !"; feu = 2; ChargerListeAllergene(ListAl); }
            }

            catch
            {//si la liste est vide alors le Try echoue (je n'ai trouvé que cette solution pour le moment)
                txtInfoAllergene.Text = "Allergenes compatibles";
                }
            /* FIN VERIFIE LES ALLERGENES */

            /* VERIFIE LES NUTRIMENTS  */
            List<Nutriment> ListNut = new List<Nutriment>();
            ListNut = produits.GetCheckNutriment();
            
           

            foreach (Nutriment n in ListNut)
            {
                try
                {
                    if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                    else
                    {
                        decimal valeur_produit = n.GetValeurProduit();
                        decimal valeur_profil = n.GetValeurProfil();
                        decimal seuil_vert = n.GetVert();
                        decimal seuil_orange = n.GetOrange();

                        
                        decimal maxVert = valeur_profil * (1+seuil_vert);
                        decimal maxOrange = valeur_profil * (1+seuil_orange);

                        //txtNutrimentList.Text += n.GetIdNutriment() + " ";

                        if (valeur_produit <= maxVert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible, mais en faible quantité ";  }
                        if (valeur_produit > maxVert && valeur_produit <= maxOrange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité ";  }
                        if (valeur_produit > maxOrange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité";  }

                            //Conditions pour les impacts
                            if (valeur_produit <= maxVert) { n.SetImpact("Faible"); }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange) { n.SetImpact("Moyen"); }
                            if (valeur_produit > maxOrange) { n.SetImpact("Important"); }


                            }
                }

                catch
                {
                    txtInfoNutriment.Text = "Nutriments compatibles";
                }
            }

                //Liste des nutriments incompatibles
                ChargerListeNutriment(ListNut);

                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
            if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
            if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

                //je recupère le LinearLayout qui contient le corp de la page et la liste de l'historique
                LinearLayout linearLayout = FindViewById<LinearLayout>(Resource.Id.produitInfoProduitSuggere);

                //Je récupère l'historique dans la base de données
                List<ProduitRecos> listeProduitReco = db.SelectProduitRecommande(IDproduit);

                //Pour chaque produitReco présent, je l'affiche
                foreach (ProduitRecos pr in listeProduitReco)
                {
                    //Je créer le LinearLayout qui contiendra la ligne
                    LinearLayout LN = new LinearLayout(this) { Id = 10 };
                    var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    LN.Orientation = Orientation.Horizontal;
                    LN.SetBackgroundColor(Color.LightGray);
                    paramL.SetMargins(0, 0, 0, 5);

                    //J'ajoute le LinearLayout
                    linearLayout.AddView(LN, paramL);

                    //Je créer le textView qui contient le nom du produit
                    TextView textView11 = new TextView(this) { Id = 1 };
                    textView11.Text = pr.GetidFamille();
                    var param11 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .8f);
                    textView11.SetTextColor(Color.Black);
                    textView11.SetTextSize(ComplexUnitType.Px, 22);
                    textView11.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                    LN.AddView(textView11, param11);

                    TextView textView21 = new TextView(this) { Id = 2 };
                    textView21.Text = pr.GetidProduit();
                    var param21 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param21.SetMargins(5, 0, 0, 0);
                    textView21.SetTextColor(Color.Black);
                    textView21.SetTextSize(ComplexUnitType.Px, 15);
                    textView21.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView21, param21);

                    TextView textView31 = new TextView(this) { Id = 3 };
                    textView31.Text = pr.GetproductName();
                    var param31 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param31.SetMargins(5, 0, 0, 0);
                    textView31.SetTextColor(Color.Black);
                    textView31.SetTextSize(ComplexUnitType.Px, 15);
                    textView31.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView31, param31);

                    TextView textView41 = new TextView(this) { Id = 4 };
                    textView41.Text = pr.GetidNutriment();
                    var param41 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param41.SetMargins(5, 0, 0, 0);
                    textView41.SetTextColor(Color.Black);
                    textView41.SetTextSize(ComplexUnitType.Px, 15);
                    textView41.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView41, param41);

                    TextView textView51 = new TextView(this) { Id = 5 };
                    textView51.Text = pr.GetidValeur();
                    var param51 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param51.SetMargins(5, 0, 0, 0);
                    textView51.SetTextColor(Color.Black);
                    textView51.SetTextSize(ComplexUnitType.Px, 15);
                    textView51.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView51, param51);

                    /************************************Vincent********************************/

                }



                //Verification si le produit est COMPLET
                //le produit contient des allergenes, il a donc été complété par open fact food
                try 
                {
                List<String> testComplet = produits.GetListAllergeneDuProduit();
                    if (testComplet[0] == "") { }
                }

            //le produit ne contien pas d'allergen, cela peut être normal, mais il doit contenir obligatoirement des nutriements
            catch
                {
                    try
                    {
                        var testComplet = produits.GetListNutrimentDuProduit();
                        if (testComplet[0] == "") { }
                    }
                    catch {
                        
                        txtInfoScan.Text = "Code barre : " + IDproduit;
                        txtInfoAllergene.Text = "Produit incomplet";
                        txtInfoNutriment.Text = "Nous ne pouvons donner un avis";
                        imgFeu.Visibility = ViewStates.Invisible;
                    }
                }

                /* FIN VERIFIE LES NUTRIMENTS */
                /* FIN DE LA VERIFICATION POUR LE CHARGEMENT PAR DEFAUT SUR LE PROFIL PERSO */


                /* EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */
                //expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewMatchAllNut);
                //SetData(out mAdapter, produits, IDTypeProfil);
                //expandableListView.SetAdapter(mAdapter);
                /* FIN EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */


                //-----------------/* BOUTON CHOIX PROFIL PERSO */ //-----------------------------------//
                btnProduitPERS.Click += delegate
            {
                LinearAllergene.RemoveAllViews();
                LinearNutriment.RemoveAllViews();
                btnProduitPERS.SetBackgroundColor(Color.LightGray);
                btnProduitFAM.SetBackgroundColor(Color.Gray);
                btnProduitINV.SetBackgroundColor(Color.Gray);

                btnProduitPERS.SetTextColor(Color.Gray);
                btnProduitFAM.SetTextColor(Color.LightGray);
                btnProduitINV.SetTextColor(Color.LightGray);

                feu = 0;
                IDTypeProfil = "PERS";
                //Aficchage des produits recommande
                //lstData = FindViewById<ListView>(Resource.Id.listViewProduitsuggere);
                //LoadData();

                /* EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */
                // dicMyMap.Remove(group[0]);
                // dicMyMap.Remove(group[1]);

                // expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewMatchAllNut);
                // SetData(out mAdapter, produits, IDTypeProfil);
                // expandableListView.SetAdapter(mAdapter);
                /* FIN EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */

                Produits produitPerso = new Produits();

                produitPerso = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergenePerso = new List<Allergene>();
                ListAllergenePerso = produitPerso.GetCheckAllergene();
                
                try
                {
                    if (ListAllergenePerso[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "Allergenes compatibles"; }
                    else { txtInfoAllergene.Text = "Allergenes incompatibles !"; feu = 2; ChargerListeAllergene(ListAllergenePerso); }
                }
                catch { txtInfoAllergene.Text = "Allergenes compatibles"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentPerso = new List<Nutriment>();
                ListNuttrimentPerso = produitPerso.GetCheckNutriment();
                
                foreach (Nutriment n in ListNuttrimentPerso)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();

                            decimal taux = valeur_profil / valeur_produit;
                            decimal maxVert = valeur_profil * (1+seuil_vert);
                            decimal maxOrange = valeur_profil * (1+seuil_orange);

                           // txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (valeur_produit <= maxVert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible, mais en faible quantité ";  }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité "; }
                            if (valeur_produit > maxOrange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité"; }

                            //Conditions pour les impacts
                            if (valeur_produit <= maxVert) { n.SetImpact("Faible"); }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange) { n.SetImpact("Moyen"); }
                            if (valeur_produit > maxOrange) { n.SetImpact("Important"); }

                        }
                    }

                    catch
                    {
                        txtInfoNutriment.Text = "Nutriments compatibles";
                    }
                }
                ChargerListeNutriment(ListNuttrimentPerso);


                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

                /************************************Vincent********************************/
                //je recupère le LinearLayout qui contient le corp de la page et la liste de l'historique
                linearLayout = FindViewById<LinearLayout>(Resource.Id.produitInfoProduitSuggere);

                //Je récupère l'historique dans la base de données
                listeProduitReco = db.SelectProduitRecommande(IDproduit);

                //Pour chaque produitReco présent, je l'affiche
                foreach (ProduitRecos pr in listeProduitReco)
                {
                    //Je créer le LinearLayout qui contiendra la ligne
                    LinearLayout LN = new LinearLayout(this) { Id = 10 };
                    var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    LN.Orientation = Orientation.Horizontal;
                    LN.SetBackgroundColor(Color.LightGray);
                    paramL.SetMargins(0, 0, 0, 5);

                    //J'ajoute le LinearLayout
                    linearLayout.AddView(LN, paramL);

                    //Je créer le textView qui contient le nom du produit
                    TextView textView11 = new TextView(this) { Id = 1 };
                    textView11.Text = pr.GetidFamille();
                    var param11 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .8f);
                    textView11.SetTextColor(Color.Black);
                    textView11.SetTextSize(ComplexUnitType.Dip, 22);
                    textView11.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                    LN.AddView(textView11, param11);

                    TextView textView21 = new TextView(this) { Id = 2 };
                    textView21.Text = pr.GetidProduit();
                    var param21 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param21.SetMargins(5, 0, 0, 0);
                    textView21.SetTextColor(Color.Black);
                    textView21.SetTextSize(ComplexUnitType.Dip, 15);
                    textView21.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView21, param21);

                    TextView textView31 = new TextView(this) { Id = 3 };
                    textView31.Text = pr.GetproductName();
                    var param31 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param31.SetMargins(5, 0, 0, 0);
                    textView31.SetTextColor(Color.Black);
                    textView31.SetTextSize(ComplexUnitType.Dip, 15);
                    textView31.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView31, param31);

                    TextView textView41 = new TextView(this) { Id = 4 };
                    textView41.Text = pr.GetidNutriment();
                    var param41 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param41.SetMargins(5, 0, 0, 0);
                    textView41.SetTextColor(Color.Black);
                    textView41.SetTextSize(ComplexUnitType.Dip, 15);
                    textView41.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView41, param41);

                    TextView textView51 = new TextView(this) { Id = 5 };
                    textView51.Text = pr.GetidValeur();
                    var param51 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param51.SetMargins(5, 0, 0, 0);
                    textView51.SetTextColor(Color.Black);
                    textView51.SetTextSize(ComplexUnitType.Dip, 15);
                    textView51.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView51, param51);

                    /************************************Vincent********************************/

                }





                };
            /* FIN  BOUTON CHOIX PROFIL PERSO   */

            btnProduitFAM.Click += delegate
            {
                LinearAllergene.RemoveAllViews();
                LinearNutriment.RemoveAllViews();
                btnProduitPERS.SetBackgroundColor(Color.Gray);
                btnProduitFAM.SetBackgroundColor(Color.LightGray);
                btnProduitINV.SetBackgroundColor(Color.Gray);

                btnProduitPERS.SetTextColor(Color.LightGray);
                btnProduitFAM.SetTextColor(Color.Gray);
                btnProduitINV.SetTextColor(Color.LightGray);

                feu = 0;
                IDTypeProfil = "FAML";
                //Aficchage des produits recommande
                //lstData = FindViewById<ListView>(Resource.Id.listViewProduitsuggere);
                //LoadData();

                /* EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */
                // dicMyMap.Remove(group[0]);
                //dicMyMap.Remove(group[1]);


                //expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewMatchAllNut);
                // SetData(out mAdapter, produits, IDTypeProfil);
                // expandableListView.SetAdapter(mAdapter);
                /* FIN EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */


                Produits produitFamille = new Produits();

                produitFamille = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneFamille = new List<Allergene>();
                ListAllergeneFamille = produitFamille.GetCheckAllergene();

                try
                {
                    if (ListAllergeneFamille[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "Allergenes compatibles"; }
                    else { txtInfoAllergene.Text = "Allergenes incompatibles !"; feu = 2; ChargerListeAllergene(ListAllergeneFamille); }
                }
                catch { txtInfoAllergene.Text = "Allergenes compatibles"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentFamille = new List<Nutriment>();
                ListNuttrimentFamille = produitFamille.GetCheckNutriment();
                

                txtInfoNutriment.Text = "Nutriments compatibles";

                foreach (Nutriment n in ListNuttrimentFamille)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "" && feu == 0) { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();

                            decimal taux = valeur_profil / valeur_produit;
                            decimal maxVert = valeur_profil * (1+seuil_vert);
                            decimal maxOrange = valeur_profil * (1+seuil_orange);

                            //txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (valeur_produit <= maxVert && feu == 0) { feu = 0; n.SetImpact("Faible"); }
                            if (valeur_produit <= maxVert) { txtInfoNutriment.Text = "Nutriment incompatible, mais en faible quantité ";  }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité ";  }
                            if (valeur_produit > maxOrange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité"; }

                            //Conditions pour les impacts
                            if (valeur_produit <= maxVert) { n.SetImpact("Faible"); }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange) { n.SetImpact("Moyen"); }
                            if (valeur_produit > maxOrange) { n.SetImpact("Important"); }
                        }
                    }

                    catch
                    {
                        if (feu == 0) { txtInfoNutriment.Text = "Nutriments compatibles"; }
                    }
                }

                //Charger la liste des nutriments
                ChargerListeNutriment(ListNuttrimentFamille);

                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

                /************************************Vincent********************************/
                //je recupère le LinearLayout qui contient le corp de la page et la liste de l'historique
                 linearLayout = FindViewById<LinearLayout>(Resource.Id.produitInfoProduitSuggere);

                //Je récupère l'historique dans la base de données
                listeProduitReco = db.SelectProduitRecommande(IDproduit);

                //Pour chaque produitReco présent, je l'affiche
                foreach (ProduitRecos pr in listeProduitReco)
                {
                    //Je créer le LinearLayout qui contiendra la ligne
                    LinearLayout LN = new LinearLayout(this) { Id = 10 };
                    var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    LN.Orientation = Orientation.Horizontal;
                    LN.SetBackgroundColor(Color.LightGray);
                    paramL.SetMargins(0, 0, 0, 5);

                    //J'ajoute le LinearLayout
                    linearLayout.AddView(LN, paramL);

                    //Je créer le textView qui contient le nom du produit
                    TextView textView11 = new TextView(this) { Id = 1 };
                    textView11.Text = pr.GetidFamille();
                    var param11 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .8f);
                    textView11.SetTextColor(Color.Black);
                    textView11.SetTextSize(ComplexUnitType.Dip, 22);
                    textView11.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                    LN.AddView(textView11, param11);

                    TextView textView21 = new TextView(this) { Id = 2 };
                    textView21.Text = pr.GetidProduit();
                    var param21 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param21.SetMargins(5, 0, 0, 0);
                    textView21.SetTextColor(Color.Black);
                    textView21.SetTextSize(ComplexUnitType.Dip, 15);
                    textView21.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView21, param21);

                    TextView textView31 = new TextView(this) { Id = 3 };
                    textView31.Text = pr.GetproductName();
                    var param31 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param31.SetMargins(5, 0, 0, 0);
                    textView31.SetTextColor(Color.Black);
                    textView31.SetTextSize(ComplexUnitType.Dip, 15);
                    textView31.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView31, param31);

                    TextView textView41 = new TextView(this) { Id = 4 };
                    textView41.Text = pr.GetidNutriment();
                    var param41 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param41.SetMargins(5, 0, 0, 0);
                    textView41.SetTextColor(Color.Black);
                    textView41.SetTextSize(ComplexUnitType.Dip, 15);
                    textView41.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView41, param41);

                    TextView textView51 = new TextView(this) { Id = 5 };
                    textView51.Text = pr.GetidValeur();
                    var param51 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param51.SetMargins(5, 0, 0, 0);
                    textView51.SetTextColor(Color.Black);
                    textView51.SetTextSize(ComplexUnitType.Dip, 15);
                    textView51.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView51, param51);

                    /************************************Vincent********************************/

                }

            };

            /* BOUTON CHOIX PROFIL INVITE  */
            btnProduitINV.Click += delegate
            {
                LinearAllergene.RemoveAllViews();
                LinearNutriment.RemoveAllViews();
                btnProduitPERS.SetBackgroundColor(Color.Gray);
                btnProduitFAM.SetBackgroundColor(Color.Gray);
                btnProduitINV.SetBackgroundColor(Color.LightGray);

                btnProduitPERS.SetTextColor(Color.LightGray);
                btnProduitFAM.SetTextColor(Color.LightGray);
                btnProduitINV.SetTextColor(Color.Gray);

                feu = 0;
                IDTypeProfil = "INVT";

                //Aficchage des produits recommande
                //lstData = FindViewById<ListView>(Resource.Id.listViewProduitsuggere);
                //LoadData();

                /* EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */
                // dicMyMap.Remove(group[0]);
                // dicMyMap.Remove(group[1]);

                // expandableListView = FindViewById<ExpandableListView>(Resource.Id.expandableListViewMatchAllNut);
                //  SetData(out mAdapter, produits, IDTypeProfil);
                //  expandableListView.SetAdapter(mAdapter);
                /* FIN EDITION DE LA LISTE DES ALLERGENE ET NUTRIMENTS QUI MATCHENT AVEC LE PROFIL */


                Produits produitInvite = new Produits();

                produitInvite = db.SelectIdProduit(IDproduit, IDTypeProfil);
                db.InsertIntoHistorique(IDTypeProfil, IDproduit);
                List<Allergene> ListAllergeneInvite = new List<Allergene>();
                ListAllergeneInvite = produitInvite.GetCheckAllergene();

                try
                {
                    if (ListAllergeneInvite[0].GetIdAlergene() == "") { txtInfoAllergene.Text = "Allergene compatibles"; }
                    else { txtInfoAllergene.Text = "Allergenes incompatibles !"; feu = 2; ChargerListeAllergene(ListAllergeneInvite); }
                }
                catch { txtInfoAllergene.Text = "Allergene compatibles"; }
                /* FIN VERIFIE LES ALLERGENES */

                /* VERIFIE LES NUTRIMENTS  */
                List<Nutriment> ListNuttrimentInvite = new List<Nutriment>();
                ListNuttrimentInvite = produitInvite.GetCheckNutriment();

                foreach (Nutriment n in ListNuttrimentInvite)
                {
                    try
                    {
                        if (n.GetIdNutriment() == "") { txtInfoNutriment.Text = "Nutriments compatibles"; }
                        else
                        {
                            decimal valeur_produit = n.GetValeurProduit();
                            decimal valeur_profil = n.GetValeurProfil();
                            decimal seuil_vert = n.GetVert();
                            decimal seuil_orange = n.GetOrange();

                            decimal taux = valeur_profil / valeur_produit;
                            decimal maxVert = valeur_profil * (1+seuil_vert);
                            decimal maxOrange = valeur_profil * (1+seuil_orange);

                            //txtNutrimentList.Text += n.GetIdNutriment() + " ";

                            if (valeur_produit <= maxVert && feu == 0) { feu = 0; txtInfoNutriment.Text = "Nutriment incompatible, mais en faible quantité ";  }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange && feu == 0) { feu = 1; txtInfoNutriment.Text = "Nutriment incompatible en moyenne quantité ";  }
                            if (valeur_produit > maxOrange && feu < 2) { feu = 2; txtInfoNutriment.Text = "Nutriment incompatible en grande quantité";  }

                            //Conditions pour les impacts
                            if (valeur_produit <= maxVert) { n.SetImpact("Faible"); }
                            if (valeur_produit > maxVert && valeur_produit <= maxOrange) { n.SetImpact("Moyen"); }
                            if (valeur_produit > maxOrange) { n.SetImpact("Important"); }
                        }
                    }

                    catch
                    {
                        txtInfoNutriment.Text = "Nutriments compatibles";
                    }
                }

                //Charger la liste des nutriments
                ChargerListeNutriment(ListNuttrimentInvite);

                /* FIN VERIFIE LES NUTRIMENTS   */
                if (feu == 0) { imgFeu.SetImageResource(Resource.Drawable.feuVertSmall); }
                if (feu == 1) { imgFeu.SetImageResource(Resource.Drawable.feuOrangeSmall); }
                if (feu == 2) { imgFeu.SetImageResource(Resource.Drawable.feurougeSmall); }

                /************************************Vincent********************************/
                //je recupère le LinearLayout qui contient le corp de la page et la liste de l'historique
                 linearLayout = FindViewById<LinearLayout>(Resource.Id.produitInfoProduitSuggere);

                //Je récupère l'historique dans la base de données
                listeProduitReco = db.SelectProduitRecommande(IDproduit);

                //Pour chaque produitReco présent, je l'affiche
                foreach (ProduitRecos pr in listeProduitReco)
                {
                    //Je créer le LinearLayout qui contiendra la ligne
                    LinearLayout LN = new LinearLayout(this) { Id = 10 };
                    var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                    LN.Orientation = Orientation.Horizontal;
                    LN.SetBackgroundColor(Color.LightGray);
                    paramL.SetMargins(0, 0, 0, 5);

                    //J'ajoute le LinearLayout
                    linearLayout.AddView(LN, paramL);

                    //Je créer le textView qui contient le nom du produit
                    TextView textView11 = new TextView(this) { Id = 1 };
                    textView11.Text = pr.GetidFamille();
                    var param11 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .8f);
                    textView11.SetTextColor(Color.Black);
                    textView11.SetTextSize(ComplexUnitType.Dip, 22);
                    textView11.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                    LN.AddView(textView11, param11);

                    TextView textView21 = new TextView(this) { Id = 2 };
                    textView21.Text = pr.GetidProduit();
                    var param21 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param21.SetMargins(5, 0, 0, 0);
                    textView21.SetTextColor(Color.Black);
                    textView21.SetTextSize(ComplexUnitType.Dip, 15);
                    textView21.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView21, param21);

                    TextView textView31 = new TextView(this) { Id = 3 };
                    textView31.Text = pr.GetproductName();
                    var param31 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param31.SetMargins(5, 0, 0, 0);
                    textView31.SetTextColor(Color.Black);
                    textView31.SetTextSize(ComplexUnitType.Dip, 15);
                    textView31.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView31, param31);

                    TextView textView41 = new TextView(this) { Id = 4 };
                    textView41.Text = pr.GetidNutriment();
                    var param41 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param41.SetMargins(5, 0, 0, 0);
                    textView41.SetTextColor(Color.Black);
                    textView41.SetTextSize(ComplexUnitType.Dip, 15);
                    textView41.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView41, param41);

                    TextView textView51 = new TextView(this) { Id = 5 };
                    textView51.Text = pr.GetidValeur();
                    var param51 = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, .2f);
                    param51.SetMargins(5, 0, 0, 0);
                    textView51.SetTextColor(Color.Black);
                    textView51.SetTextSize(ComplexUnitType.Dip, 15);
                    textView51.SetTypeface(Typeface.Default, TypefaceStyle.Normal);

                    LN.AddView(textView51, param51);

                    /************************************Vincent********************************/

                }
            };

        }//FIN DU ELSE PRODUIT NON TROUVE


            /*  MENU DU BAS  */

            //Lorsque l'on clique sur le bouton menuProfil
            menuProfil.Click += delegate
            {
                StartActivity(typeof(Profil));
            };

            //Lorsque l'on clique sur le bouton menuHistorique
            menuHistorique.Click += delegate
            {
                StartActivity(typeof(Historique));
            };


            //Lorsque l'on clique sur le bouton menuConseilMoi
            menuConseil.Click += delegate
            {
                StartActivity(typeof(Conseil));
            };

            //Clik sur le bouton scanner
            menuScanner.Click += async (sender, e) =>
            {
                //on active le lecteur code barre et on attend une réponse (un code barre est lu)
                var scanner = new ZXing.Mobile.MobileBarcodeScanner();

                //un code bare est lu, le résultat va dans la variable résult
                var result = await scanner.Scan();
                if (result != null)
                {
                    //Intent stocke la variable ID Produit et la transmet à l'activité Produit
                    Intent produit = new Intent(this, typeof(Produit));
                    produit.PutExtra("IDproduit", result.Text);
                    StartActivity(produit);
                }
                else { }
            };

            //Lorsque l'on clique sur menuAvertissement
            menuAvertissement.Click += delegate
            {
                StartActivity(typeof(Avertissement));
            };
        }

        //VINCENT -> A expliquer
        private void LoadData()
        {
            //lstSource = db1.SelectProduitRecommande("80051657");
            //var adapter = new ListViewAdapterProduitRecommandation(this, lstSource);
            //lstData.Adapter = adapter;
        }


        private void SetData(out ExpandableListViewAdapter_DetailProduit mAdapter, Produits p, String tp)
        {

            p = db.SelectIdProduit(IDproduit, tp);
            //dicMyMap = null;
            List<string> groupA = new List<string>();
            //groupA.Add("A-1");


            List<string> groupB = new List<string>();
            //groupB.Add("B-1");


            List<Allergene> ListAll = new List<Allergene>();
            ListAll = p.GetCheckAllergene();

            foreach (Allergene a in ListAll)
            {
                groupA.Add(db.GetLibAllergene( a.GetIdAlergene()));

            }

            List<Nutriment> ListNut = new List<Nutriment>();
            ListNut = p.GetCheckNutriment();

            foreach (Nutriment n in ListNut)
            {
                groupB.Add(db.GetLibNutriment(n.GetIdNutriment()));

            }

            dicMyMap.Add(group[0], groupA);
            dicMyMap.Add(group[1], groupB);

            mAdapter = new ExpandableListViewAdapter_DetailProduit(this, group, dicMyMap);

        }

        private void ChargerListeAllergene(List<Allergene> ListAl)
        {
            LinearLayout LinearAllergene = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMatchAllergene);
            db.ExistBase(this);
            LinearAllergene.RemoveAllViews();
            foreach (Allergene a in ListAl)
            { 
                TextView TextViewAllergene = new TextView(this) { Id = 1 };
                TextViewAllergene.Text = db.GetLibAllergene(a.GetIdAlergene());
                TextViewAllergene.SetTextSize(ComplexUnitType.Dip, 15);
                TextViewAllergene.SetTextColor(Color.DarkGray);
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                LinearAllergene.AddView(TextViewAllergene, param);
            }//ForEach
        }//Fin ChargerListeAllergene

        private void ChargerListeNutriment(List<Nutriment> ListNut)
        {
            LinearLayout LinearNutriment = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMatchNutriment);
            db.ExistBase(this);
            LinearNutriment.RemoveAllViews();
            foreach (Nutriment n in ListNut)
            {
                TextView TextViewNutriment = new TextView(this) { Id = 2 };
                TextViewNutriment.Text = db.GetLibNutriment(n.GetIdNutriment())+" - " +n.GetImpact();
                TextViewNutriment.SetTextSize(ComplexUnitType.Dip, 15);
                TextViewNutriment.SetTextColor(Color.DarkGray);
                var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                LinearNutriment.AddView(TextViewNutriment, param);

            }//Fin Foreach
        }//fin ChargerListeNutriment

    }
}