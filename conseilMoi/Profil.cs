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
            txtViewTitre.SetTextSize(ComplexUnitType.Px, 24);
            txtViewTitre.SetTypeface(Typeface.Default, TypefaceStyle.Bold);


            btnProfilPERS.SetTextSize(ComplexUnitType.Px, 22);
            btnProfilPERS.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

            btnProfilFAML.SetTextSize(ComplexUnitType.Px, 22);
            btnProfilFAML.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

            btnProfilINVT.SetTextSize(ComplexUnitType.Px, 22);
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
                textView1.SetTextAppearance(this, Android.Resource.Style.TextAppearanceLarge);
                
                textView1.SetTextColor(Color.Black);
                textView1.SetTextSize(ComplexUnitType.Px, 22);
                textView1.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                textView1.SetBackgroundColor(Color.LightCyan);
                textView1.SetHeight(30);
              //textView1.SetWidth(0);
              //Je définis les paramètres du textView
              var param = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent, 1f);
                param.SetMargins(5,10,0,0);
                //J'ajoute le TextView tout bien rempli à mon LinearLayout
                linearLayout.AddView(textView1, param);

                //Je déclare la liste des sous-groupes
                List<ProfilsStandards> groupeCriteres = new List<ProfilsStandards>();
                //Je rempli la liste de tous les sous-groupes avec la BDD qu'il est possible de choisir
                groupeCriteres = db.SelectcritereProfilstandard(p.GetIdProfil());
                //Je créer le LinearLayout qui contiendra cette liste de sous-groupes, avec ses paramètres
                LinearLayout LN = new LinearLayout(this) { Id = 5 };
                var paramL = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
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
                            var paramLh = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            linearLigne.Orientation = Orientation.Horizontal;
                            linearLayout.SetBackgroundColor(Color.LightGray);

                            //Je créer la CheckBox
                            CheckBox checkBox = new CheckBox(this) { Id = a*100+b };
                            b++;
                            checkBox.SetHighlightColor(Color.Black);
                           
                            checkBox.Text = db.GetNomCritereFromIdCritere(ps.GetidCritere());
                            checkBox.Gravity = GravityFlags.CenterVertical;
                            checkBox.SetTextColor(Color.Black);
                            var param2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            linearLigne.AddView(checkBox, param);

                            //Je créer la case valeur
                            TextView valeur = new TextView(this) { Id = 7 };
                            var paramValeur = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                            paramValeur.SetMargins(10,10,0,0);
                            valeur.Gravity = GravityFlags.CenterVertical;
                            valeur.Text = val+"/100gr";
                            //valeur.SetTextSize(ComplexUnitType.Px, 16);
                            valeur.SetTextColor(Color.Black);
                            linearLigne.AddView(valeur, paramValeur);

                            //Je créer les boutons + et -
                            ImageView plus = new ImageView(this) { Id = 8 };
                            ImageView moins = new ImageView(this) { Id = 9 };
                            
                            var paramPlus = new LinearLayout.LayoutParams(15, 15);
                            var paramMoins = new LinearLayout.LayoutParams(15, 15);
                            paramPlus.Gravity = GravityFlags.CenterVertical;
                            paramPlus.SetMargins(10, 5, 0, 0);
                            paramMoins.Gravity = GravityFlags.CenterVertical;
                            paramMoins.SetMargins(10, 5, 0, 0);

                            plus.SetImageResource(Resource.Drawable.plus);
                            moins.SetImageResource(Resource.Drawable.moins);

                            valeur.Visibility = ViewStates.Invisible;
                            plus.Visibility = ViewStates.Invisible;
                            moins.Visibility = ViewStates.Invisible;

                            linearLigne.AddView(plus, paramPlus);
                            linearLigne.AddView(moins, paramMoins);

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
                                    val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());
                                    valeur.Text = val + "/100gr";
                                }
                            }

                            checkBox.Click += delegate
                            {
                                if (checkBox.Checked == true) {
                                    db.InsertProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil(), ps.GetValeur() );
                                    if (db.EstUnAllergene(ps.GetidCritere()) == false)
                                    {
                                        valeur.Visibility = ViewStates.Visible;
                                        plus.Visibility = ViewStates.Visible;
                                        moins.Visibility = ViewStates.Visible;
                                        val = db.GetValeurProfilUtilisateur(ps.GetidCritere(), ID_typeProfil, ps.GetidProfil());
                                        valeur.Text = val + "/100gr";
                                        val = db.GetValeurProfilStansard(ps.GetidCritere(), ps.GetidProfil());
                                    }

                                 }
                                    

                                if (checkBox.Checked == false) {
                                    db.DeleteProfilUtilisateur(ID_typeProfil, p.GetIdProfil(), ps.GetidCritere());
                                    valeur.Visibility = ViewStates.Invisible;
                                    plus.Visibility = ViewStates.Invisible;
                                    moins.Visibility = ViewStates.Invisible;
                                }
                            };

                            plus.Click += delegate
                            {
                                val = db.GetValeurProfilStansard(ps.GetidCritere(), ps.GetidProfil());
                                val++;
                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val);
                            };

                            moins.Click += delegate
                            {
                                val = db.GetValeurProfilStansard(ps.GetidCritere(), ps.GetidProfil());
                                val--;
                                valeur.Text = val + "/100gr";
                                db.UpdateValeur(ps.GetidCritere(), ID_typeProfil, p.GetIdProfil(), val);
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