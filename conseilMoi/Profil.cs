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
using conseilMoi.Classes;
//using Android.Support.V7.Widget;
using Android.Support.V7.App;
using Android.Graphics;
using Android.Util;


namespace conseilMoi
{
    [Activity(Label = "Profil" /*Icon = "@drawable/logo"*/)]
    public class Profil : Activity
    {
       ListView lstData;
        List<Profils> lstSource = new List<Profils>();
        MaBase db = new MaBase();
        LinearLayout linearLayout;

        ExpandableListViewAdapter mAdapter;
        ExpandableListView expandableListView;
        List<string> group = new List<string>();

        Dictionary<string, List<string>> dicMyMap = new Dictionary<string, List<string>>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            RequestWindowFeature(WindowFeatures.NoTitle);
            base.OnCreate(savedInstanceState);
           
            db.ExistBase(this);

            SetContentView(Resource.Layout.Profil);

            MobileBarcodeScanner.Initialize(Application);

            var txtViewTitre = FindViewById<TextView>(Resource.Id.textView1);
            var menuProfil = FindViewById<ImageView>(Resource.Id.imageViewProfilProfil);
            var menuHistorique = FindViewById<ImageView>(Resource.Id.imageViewProfilHistorique);
            var menuScanner = FindViewById<ImageView>(Resource.Id.imageViewProfilScann);
            var menuConseil = FindViewById<ImageView>(Resource.Id.imageViewProfilConseil);
            var menuAvertissement = FindViewById<ImageView>(Resource.Id.imageViewProfilAvertissement);
            linearLayout = FindViewById<LinearLayout>(Resource.Id.linearLayoutProfile);
           
            Button btnProfilPERS = FindViewById<Button>(Resource.Id.buttonProfilPERS);
            Button btnProfilFAML = FindViewById<Button>(Resource.Id.buttonProfilFAML);
            Button btnProfilINVT = FindViewById<Button>(Resource.Id.buttonProfilINVT);

            btnProfilINVT.Text = "Mes amis";

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


            /* NOUVELLE VERSION LIONEL - AFFICHAGE DES GROUPES DE PROFIL - NOUVELLE VERSION LIONEL -- NOUVELLE VERSION LIONEL --   */
            txtViewTitre.SetTextSize(ComplexUnitType.Dip, 26);
            txtViewTitre.SetTypeface(Typeface.Default, TypefaceStyle.Bold);


            btnProfilPERS.SetTextSize(ComplexUnitType.Dip, 24);
            btnProfilPERS.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

            btnProfilFAML.SetTextSize(ComplexUnitType.Dip, 24);
            btnProfilFAML.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

            btnProfilINVT.SetTextSize(ComplexUnitType.Dip, 24);
            btnProfilINVT.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

            btnProfilPERS.SetBackgroundColor(Color.LightGray);
            btnProfilFAML.SetBackgroundColor(Color.DarkGray);
            btnProfilINVT.SetBackgroundColor(Color.DarkGray);

            btnProfilPERS.SetTextColor(Color.DarkGray);
            btnProfilFAML.SetTextColor(Color.LightGray);
            btnProfilINVT.SetTextColor(Color.LightGray);

            MakeData("PERS");

            btnProfilPERS.Click += delegate
            {

                btnProfilPERS.SetBackgroundColor(Color.LightGray);
                btnProfilFAML.SetBackgroundColor(Color.DarkGray);
                btnProfilINVT.SetBackgroundColor(Color.DarkGray);

                btnProfilPERS.SetTextColor(Color.DarkGray);
                btnProfilFAML.SetTextColor(Color.LightGray);
                btnProfilINVT.SetTextColor(Color.LightGray);

                linearLayout.RemoveAllViews();
                MakeData("PERS");


            };

            btnProfilFAML.Click += delegate
            {
                btnProfilPERS.SetBackgroundColor(Color.DarkGray);
                btnProfilFAML.SetBackgroundColor(Color.LightGray);
                btnProfilINVT.SetBackgroundColor(Color.DarkGray);

                btnProfilPERS.SetTextColor(Color.LightGray);
                btnProfilFAML.SetTextColor(Color.DarkGray);
                btnProfilINVT.SetTextColor(Color.LightGray);

                linearLayout.RemoveAllViews();
                MakeData("FAML");
            };

            btnProfilINVT.Click += delegate
            {
                btnProfilPERS.SetBackgroundColor(Color.DarkGray);
                btnProfilFAML.SetBackgroundColor(Color.DarkGray);
                btnProfilINVT.SetBackgroundColor(Color.LightGray);

                btnProfilPERS.SetTextColor(Color.LightGray);
                btnProfilFAML.SetTextColor(Color.LightGray);
                btnProfilINVT.SetTextColor(Color.DarkGray);

                linearLayout.RemoveAllViews();
                MakeData("INVT");
            };




            /*FIN NOUVELLE VERSION LIONEL -- FIN NOUVELLE VERSION LIONEL -- FIN NOUVELLE VERSION LIONEL --   */
        }


        //GENERER LES GROUPES ET SOUS GROUPES DE PROFILS -------------------------------------------------------------
        public void MakeData(String ID_typeProfil)
        {
            //Chargement du linearLayout à remplir

            db.ExistBase(this);
            //je déclare la liste des groupes de profil
            List<Profils> groupProfils = new List<Profils>();
            groupProfils = db.SelectNomProfil();
            int a = 1;
            int b = 1;
            //pour caque groupe de profil(Sportif, Allergique ...etc), on l'afficher et on  recherche ses sous-groupes
            foreach (Profils p in groupProfils)
            {
                //JE créer le TextView du Groupe
                TextView textView1 = new TextView(this) { Id = a };
                a++;
                //Je rempli le texte (Exemple (Sportif)
                textView1.Text = p.GetNomProfil();
                //Je définis la couleur à "Black" car c'est blanc par défaut
                //textView1.SetTextAppearance(this, Android.Resource.Style.TextAppearanceLarge);
                
                textView1.SetTextColor(Color.Black);
                textView1.SetTextSize(ComplexUnitType.Dip, 30);
                textView1.SetTextSize(ComplexUnitType.Dip, 30);
                textView1.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                textView1.SetBackgroundColor(Color.LightCyan);
                //textView1.SetHeight(30);
              //textView1.SetWidth(0);
              //Je définis les paramètres du textView
              var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                param.SetMargins(5,10,0,0);
                //J'ajoute le TextView tout bien rempli à mon LinearLayout
                linearLayout.AddView(textView1, param);

                //Je déclare la liste des sous-groupes
                List<ProfilsStandards> groupeCriteres = new List<ProfilsStandards>();
                //Je rempli la liste de tous les sous-groupes avec la BDD qu'il est possible de choisir
                groupeCriteres = db.SelectcritereProfilstandard(p.GetIdProfil());
                //Je créer le LinearLayout qui contiendra cette liste de sous-groupes, avec ses paramètres
                LinearLayout LN = new LinearLayout(this) { Id = 5 };
                var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                LN.Orientation = Orientation.Vertical;
                //J'ajoute le LinearLayout
                linearLayout.AddView(LN);

                //variable pour vérifier si le groupe est ouvert ou fermé
                bool ouvert = false;

                //Si l'on clique sur le Groupe, on ouvre les sous groupes
                textView1.Click += delegate
                {
                    //Si le groupe est à l'état de "fermé", on va l'ouvrir
                    if (ouvert == false)
                    {
                        ouvert = true;
                        //Pour chaque sous-goupe dans la liste, on va l'afficher !
                        foreach (ProfilsStandards ps in groupeCriteres)
                        {
                            Decimal val = 0;
                            //Le créer le LinearLayout qui contient (Horizontalement) la CheckBox et la valeur
                            LinearLayout linearLigne = new LinearLayout(this) { Id = 6 };
                            var paramLh = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                            linearLigne.Orientation = Orientation.Vertical;
                            linearLayout.SetBackgroundColor(Color.LightGray);

                            //Je créer la CheckBox
                            CheckBox checkBox = new CheckBox(this) { Id = a*100+b };
                            b++;
                            checkBox.SetHighlightColor(Color.Black);
                           
                            checkBox.Text = db.GetNomCritereFromIdCritere(ps.GetidCritere());
                            checkBox.Gravity = GravityFlags.CenterVertical;
                            checkBox.SetTextColor(Color.Black);
                            checkBox.SetTextSize(ComplexUnitType.Dip, 28);
                            var param2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                            linearLigne.AddView(checkBox, param);


                            LinearLayout linearQTE = new LinearLayout(this) { Id = 11 };
                            var paramLQTE = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, 0);
                            linearQTE.Orientation = Orientation.Horizontal;
                            linearQTE.SetGravity ( GravityFlags.CenterHorizontal);

                           // var taille = ComplexUnitType.Dip = 15;
                            //Je créer la case valeur
                            TextView valeur = new TextView(this) { Id = 7 };
                            var paramValeur = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            paramValeur.SetMargins(50,0,0,0);
                            valeur.Gravity = GravityFlags.CenterVertical;

                            valeur.Text = val+"/100gr";
                            valeur.SetTextSize(ComplexUnitType.Dip, 22);

                            
                            
                            valeur.SetTextColor(Color.Black);
                            linearQTE.AddView(valeur, paramValeur);

                            /*
                            Button plus = new Button(this) { Id = 108 };
                            Button plusPlus = new Button(this) { Id = 18 };
                            Button moins = new Button(this) { Id = 109 };
                            Button moinsMoins = new Button(this) { Id = 19 };*/

                            //Je créer les boutons + et -
                             ImageView plus = new ImageView(this) { Id = 108 };
                             ImageView plusPlus = new ImageView(this) { Id = 18 };
                             ImageView moins = new ImageView(this) { Id = 109 };
                             ImageView moinsMoins = new ImageView(this) { Id =19 };


                            /*
                            plus.Text = "+ 1";
                            plusPlus.Text = "+ 0.1";
                            moins.Text = "- 0.1";
                            moinsMoins.Text = "- 1";
                            plus.SetTextSize(ComplexUnitType.Px, 12);
                            plusPlus.SetTextSize(ComplexUnitType.Px, 12);
                            moins.SetTextSize(ComplexUnitType.Px, 12);
                            moinsMoins.SetTextSize(ComplexUnitType.Px, 12);*/

                            var paramPlus = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 0.2f);
                            var paramPlusPlus = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 0.2f);
                            var paramMoins = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 0.2f);
                            var paramMoinsMoins = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 0.2f);

                            paramPlus.Gravity = GravityFlags.CenterVertical;
                            paramPlusPlus.Gravity = GravityFlags.CenterVertical;
                            paramPlus.SetMargins(5, 0, 5, 0);
                            paramPlusPlus.SetMargins(5, 0, 5, 0);
                            paramMoins.Gravity = GravityFlags.CenterVertical;
                            paramMoinsMoins.Gravity = GravityFlags.CenterVertical;
                            paramMoins.SetMargins(5, 0, 5, 0);
                            paramMoinsMoins.SetMargins(5, 0, 5, 0);

                            plus.SetImageResource(Resource.Drawable.plus1);
                            plusPlus.SetImageResource(Resource.Drawable.plusplus1);
                            moins.SetImageResource(Resource.Drawable.moinsmoins1);
                            moinsMoins.SetImageResource(Resource.Drawable.moins1);

                            valeur.Visibility = ViewStates.Invisible;
                            plus.Visibility = ViewStates.Invisible;
                            plusPlus.Visibility = ViewStates.Invisible;
                            moins.Visibility = ViewStates.Invisible;
                            moinsMoins.Visibility = ViewStates.Invisible;

                            linearQTE.AddView(plus, paramPlus);
                            linearQTE.AddView(plusPlus, paramPlusPlus);
                            linearQTE.AddView(moins, paramMoins);
                            linearQTE.AddView(moinsMoins, paramMoinsMoins);

                            linearLigne.AddView(linearQTE, paramLQTE);

                            //j'ajoute le LinearLayout Ligne au LinearLayout
                            LN.AddView(linearLigne, paramLh);

                            //Je check la checkBox, ou pas
                            String res = db.VerifProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil());
                            string[] words = res.Split(' ');
                            if (words[0] == "check") {
                                checkBox.Checked = true;

                                

                                try { val = Decimal.Parse(words[3]); } catch { }
                                if (db.EstUnAllergene(ps.GetidCritere()) == false)
                                {
                                    valeur.Visibility = ViewStates.Visible;
                                    plus.Visibility = ViewStates.Visible;
                                    moins.Visibility = ViewStates.Visible;
                                    plusPlus.Visibility = ViewStates.Visible;
                                    moinsMoins.Visibility = ViewStates.Visible;
                                    val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());
                                    valeur.Text = val + "/100gr";

                                    paramLQTE = new LinearLayout.LayoutParams( LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent); //Width, Height
                                    linearQTE.LayoutParameters = paramLQTE;
                                }
                            }
                            

                            checkBox.Click += delegate
                            {
                                if (checkBox.Checked == true) {

                                    String ValInsert = ps.GetValeur().Replace(',', '.');
                                    //insertion du critère pour le profil utilisateur
                                    String retour = db.InsertProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil(), ValInsert.Replace(',','.'));
                                    //checkBox.Text += " " +retour+" "+ ValInsert;
                                    if (db.EstUnAllergene(ps.GetidCritere()) == false)
                                    {
                                        valeur.Visibility = ViewStates.Visible;
                                        plus.Visibility = ViewStates.Visible;
                                        moins.Visibility = ViewStates.Visible;
                                        plusPlus.Visibility = ViewStates.Visible;
                                        moinsMoins.Visibility = ViewStates.Visible;
                                        val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());
                                        valeur.Text = val + "/100gr";
                                        val = db.GetValeurProfilStansard(ps.GetidCritere(), ps.GetidProfil());

                                        paramLQTE = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, LinearLayout.LayoutParams.WrapContent); //Width, Height
                                        linearQTE.LayoutParameters = paramLQTE;
                                    }

                                 }
                                    

                                if (checkBox.Checked == false) {

                                    paramLQTE = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.WrapContent, 0); //Width, Height
                                    linearQTE.LayoutParameters = paramLQTE;

                                    db.DeleteProfilUtilisateur(ID_typeProfil, p.GetIdProfil(), ps.GetidCritere());
                                    valeur.Visibility = ViewStates.Invisible;
                                    valeur.Visibility = ViewStates.Invisible;
                                    plus.Visibility = ViewStates.Invisible;
                                    plusPlus.Visibility = ViewStates.Invisible;
                                    moins.Visibility = ViewStates.Invisible;
                                    moinsMoins.Visibility = ViewStates.Invisible;
                                }
                            };

                            plus.Click += delegate
                            {
                                //Je récupére la valeur dans la table profil utilisateur
                                val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());
                                //Je créer une valiable décimal pour faire l'oppération
                                
                                val++;
                                if (val > 100) { val = 100; }

                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val.ToString().Replace(',','.'));
                            };

                            moins.Click += delegate
                            {
                                val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());

                                val = val - 0.1M;
                                if (val < 0) { val = 0; }
;
                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val.ToString().Replace(',', '.'));
                            };
                            plusPlus.Click += delegate
                            {
                                val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());

                                val = val + 0.1M;
                                if (val > 100) { val = 100; }

                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val.ToString().Replace(',', '.'));
                            };

                            moinsMoins.Click += delegate
                            {
                                //decimal i = 1.1M;

                                val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());

                                val--;
                                if (val < 0) { val = 0; }

                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val.ToString().Replace(',', '.'));
                            };

                        }
                    } //FIN SI
                    //Sinon, s'il est ouvert, on le ferme
                    else
                    {
                        ouvert = false;
                        LN.RemoveAllViews();
                    } //FIN else
                }; //FIN textView1.Click
                b = 1;
            } //Fin For Each

        }// Fin MakeData

        //FIN GENERER LES GROUPES ET SOUS GROUPES DE PROFILS--------------------------------------------------------------

        private void LoadData()
        {
            lstSource = db.SelectNomProfil();
            var adapter = new ListViewAdapterProfil(this, lstSource);
            lstData.Adapter = adapter;
        }

    }
}