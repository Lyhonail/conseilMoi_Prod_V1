﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Mono.Data.Sqlite;
using System.IO;
using Android.Database.Sqlite;
using Android.Util;
using conseilMoi.Resources.Classes;
using conseilMoi.Classes;

namespace conseilMoi.Resources.MaBase
{
    class MaBase
    {
        //déclaration des variables de la classe
        String path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); //chemin d'enregistrement de la base
        String maBase; //contient 'path' + le nom de la base - utilisé pour la connexion -
        SqliteConnection connexion; //variable de connexion à la base
                                    //Fin de déclarartion des variables
        Stream fichier;

        public string ExistBase(Activity activity) //On vérifie si la base existe ( cette fonction est optionnelle car ne sert à rien d'autre)
        {
            try //si pas d'erreur
            {
                maBase = path + "/maBase.sqlite";
                //on regarde si le fichier existe déjà
                if (File.Exists(maBase)) { return maBase + " Existe déjà"; }
                else
                {
                    CreerBase(activity.Resources.OpenRawResource(Resource.Raw.data), maBase);
                    return maBase + " - à été créé";
                }
            }
            catch
            { //si erreur
                return " ERREUR";
            }
        }//fin existBase

        //On initialise le chemin de la base( cette fonction est utilisée dans les profils)
        public string ExistBase() 
        {
            try //si pas d'erreur
            {
                maBase = path+ "/maBase.sqlite"; ;
                //on regarde si le fichier existe déjà
                if (File.Exists(maBase)) { return maBase + " Existe déjà"; }
                else { return maBase + " - à été créé"; }
            }
            catch
            { //si erreur
                return " ERREUR";
            }
        }//fin existBase




        public void CreerBase(Stream resStream, string basePath)
        {
            using (resStream)
            {
                using (FileStream f = new FileStream(basePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    int bytesCount = 1024;
                    byte[] bytes = new byte[bytesCount];
                    int bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    while (bytesReaded > 0)
                    {
                        f.Write(bytes, 0, bytesReaded);
                        bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    }
                }
            }
        }

        public void ReCreerBase(Stream resStream, string basePath)
        {
            using (resStream)
            {
                using (FileStream f = new FileStream(basePath, FileMode.Create, FileAccess.Write))
                {
                    int bytesCount = 1024;
                    byte[] bytes = new byte[bytesCount];
                    int bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    while (bytesReaded > 0)
                    {
                        f.Write(bytes, 0, bytesReaded);
                        bytesReaded = resStream.Read(bytes, 0, bytesCount);
                    }
                }
            }
        }


        public void ConnexionOpen() // Ouverture de la connexion (si la base n'existe pas elle est automatiquement créée)
        {
            this.connexion = new SqliteConnection("Data Source=" + maBase + ";Version=3;");
            this.connexion.Open();
        }//fin ConnexionOpen

        public void ConnexionClose() //fermeture de la connexion
        {
            connexion.Close();
        }//fin ConnexionClose


        public string CreerTableUtilisateur()//création d'une table
        {
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "create table UTILISATEUR		" +
                                        " ( " +
                                        " ID_utilisateur integer PRIMARY KEY ASC, " +
                                        " login          text, " +
                                        " password       text, " +
                                        " nom            text, " +
                                        " prenom         text, " +
                                        " Adresse1       text, " +
                                        " Adresse2       text, " +
                                        " Ville          text, " +
                                        " CP             text, " +
                                        " TEL            text " +
                                        " ); ";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok creer table utilisateur";
            }
            //Retourne le message d'erreur SQL
            catch (SqliteException ex)
            {
                return ex.Message;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTable

        //création de la table PRofil
        public string CreerTableProfil()
        {
            try
            {
                this.ConnexionOpen();
                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "create table PROFIL		" +
                                        " ( " +
                                        " ID_profil PRIMARY KEY ASC, " +
                                        " Lib_profil          text, " +
                                        " ); ";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok creer table profil";
            }
            //Retourne le message d'erreur SQL
            catch (SqliteException ex)
            {
                return ex.Message;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil


        /*public String SelectIdProduittest()
        {
            this.ConnexionOpen();
            string sql = "select * from historique; ";
            SqliteCommand commanda = new SqliteCommand(sql, connexion);
            SqliteDataReader result = commanda.ExecuteReader();
            result.Read();

            return result.GetString(0).ToString();
        }*/

        //chargement du produit
        public Produits SelectIdProduit(String p, string IDTP)
        {
            /*-------------------------------------------------------*/
            /*---Petit code qui prend en compte PERS ET FAML si on clique sur INVT Etc...---*/
            /*-------------------------------------------------------*/
            String IdTpA = "";
            String IdTpN = "";

            if (IDTP == "PERS") {
                IdTpA = "'PERS'" ;


            }
            if (IDTP == "FAML")
            {
                IdTpA = "'FAML' " +
                    "OR PU.ID_critere = CA.ID_allergene " +
                    "AND CA.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='PERS' " +
                    "GROUP BY PU.ID_critere";
            }
            if (IDTP == "INVT") {
                IdTpA = "'INVT'"+
                     "OR PU.ID_critere = CA.ID_allergene " +
                    "AND CA.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='FAML' " +

                    "OR PU.ID_critere = CA.ID_allergene " +
                    "AND CA.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='PERS' " +
                    "GROUP BY PU.ID_critere";

            }

            if (IDTP == "PERS") { IdTpN = "'PERS'"; }
            if (IDTP == "FAML")
            {
                IdTpN = "'FAML' " +
                     "OR PU.ID_critere = CN.ID_nutriment " +
                    "AND CN.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='PERS' " +

                    "GROUP BY PU.ID_critere";
            }
            if (IDTP == "INVT") {
                IdTpN = "'INVT' " +
                     "OR PU.ID_critere = CN.ID_nutriment " +
                    "AND CN.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='PERS' " +

                    "OR PU.ID_critere = CN.ID_nutriment " +
                    "AND CN.id_produit = '" + p + "'  " +
                    " AND PU.ID_typeProfil='FAML' " +
                    "GROUP BY PU.ID_critere";
            }
            /*-------------------------------------------------------*/

            //  if (IdTp == "INVT") { IdTp = "FAML' AND PU.ID_typeProfil='PERS' AND PU.ID_typeProfil='INVT"; }
            try
            {
                this.ConnexionOpen();
                //Selection du produit
                string sql = "select id_produit, product_name, generic_name, image_small_url from produit where id_produit = " + p + "; ";
                SqliteCommand commanda = new SqliteCommand(sql, connexion);
                SqliteDataReader result = commanda.ExecuteReader();
                result.Read();
                Produits produits = new Produits();
                produits.SetProduits(result.GetString(0), result.GetString(1), result.GetString(2));
                produits.SetUrl(result.GetString(3));
                result.Close();

                //recherche de tous les allergenes qui composent le produit
                string sql_allergene = "select id_allergene from compo_allergene where id_produit  = '" + p + "'; ";
                SqliteCommand command_allergene = new SqliteCommand(sql_allergene, connexion);
                SqliteDataReader result_allergene = command_allergene.ExecuteReader();
                while (result_allergene.Read())
                {
                    produits.AddAllergene(result_allergene.GetInt32(0).ToString());
                    //produits.AddAllergene("100");
                }
                result_allergene.Close();

                //recherche de tous les nutriments qui composent le produit
                string sql_nutriment = "select id_nutriment from compo_nutriment where id_produit  = '" + p + "'; ";
                SqliteCommand command_nutriment = new SqliteCommand(sql_nutriment, connexion);
                SqliteDataReader result_nutriment = command_nutriment.ExecuteReader();
                while (result_nutriment.Read())
                {
                    produits.AddNutriment(result_nutriment.GetString(0));
                }
                result_nutriment.Close();

                /* MATCH ALLERGENE */



                //Recherche d'allergène qui matchent avec les critères du profil
                string sql_recherche_allergene =
                        " select PU.ID_typeProfil, PU.ID_profil, CAT.ID_allergene "+
                        " from profil_utilisateur PU, compo_allergene CA,   allergene_par_cat CAT "+
                        " where CAT.ID_allergene = CA.ID_allergene " +
                        " AND CAT.ID_allergene = PU.ID_critere " +
                        " AND CA.id_produit = '" + p + "' AND PU.ID_typeProfil='PERS' "+
                        " or " +
                         " CA.id_produit = '" + p + "' AND PU.ID_typeProfil = 'PERS'" +
                         " AND CAT.ID_cat_allergene = PU.ID_critere" +
                         " AND CA.ID_allergene = CAT.ID_allergene ";

                if (IDTP == "FAML")
                {
                    sql_recherche_allergene += " UNION "+
                        " select PU.ID_typeProfil, PU.ID_profil, CAT.ID_allergene " +
                        " from profil_utilisateur PU, compo_allergene CA,   allergene_par_cat CAT " +
                        " where CAT.ID_allergene = CA.ID_allergene " +
                        " AND CAT.ID_allergene = PU.ID_critere " +
                        " AND CA.id_produit = '" + p + "' AND PU.ID_typeProfil='FAML' " +
                        " or " +
                         " CA.id_produit = '" + p + "' AND PU.ID_typeProfil = 'FAML'" +
                         " AND CAT.ID_cat_allergene = PU.ID_critere" +
                         " AND CA.ID_allergene = CAT.ID_allergene ";
                }

                if ( IDTP == "INVT")
                {
                    sql_recherche_allergene += " UNION " +
                        " select PU.ID_typeProfil, PU.ID_profil, CAT.ID_allergene " +
                        " from profil_utilisateur PU, compo_allergene CA,   allergene_par_cat CAT " +
                        " where CAT.ID_allergene = CA.ID_allergene " +
                        " AND CAT.ID_allergene = PU.ID_critere " +
                        " AND CA.id_produit = '" + p + "' AND PU.ID_typeProfil='FAML' " +
                        " or " +
                         " CA.id_produit = '" + p + "' AND PU.ID_typeProfil = 'FAML'" +
                         " AND CAT.ID_cat_allergene = PU.ID_critere" +
                         " AND CA.ID_allergene = CAT.ID_allergene ";
                    sql_recherche_allergene += " UNION " +
                        " select PU.ID_typeProfil, PU.ID_profil, CAT.ID_allergene " +
                        " from profil_utilisateur PU, compo_allergene CA,   allergene_par_cat CAT " +
                        " where CAT.ID_allergene = CA.ID_allergene " +
                        " AND CAT.ID_allergene = PU.ID_critere " +
                        " AND CA.id_produit = '" + p + "' AND PU.ID_typeProfil='INVT' " +
                        " or " +
                         " CA.id_produit = '" + p + "' AND PU.ID_typeProfil = 'INVT'" +
                         " AND CAT.ID_cat_allergene = PU.ID_critere" +
                         " AND CA.ID_allergene = CAT.ID_allergene ";
                }


                SqliteCommand command_recherche_allergene = new SqliteCommand(sql_recherche_allergene, connexion);
                SqliteDataReader result_recherche_allergene = command_recherche_allergene.ExecuteReader();
                while (result_recherche_allergene.Read())
                {
                    produits.AddCheckAllergene(result_recherche_allergene.GetInt32(2).ToString(),
                                               result_recherche_allergene.GetString(0).ToString(),
                                               result_recherche_allergene.GetString(1).ToString());
                }
                //recherches d'allergenes par catégorie
                string sqlAllergeneParCat = "";


                //Fin recherche d'allergene
                result_recherche_allergene.Close();
                /* FIN MATCH ALLERGENE */

                /* MATCH NUTRIMENT */
                //Recherche de NUTRIMENTS qui matchent avec les critères du profil
                string sql_recherche_nutriment =
                    " select id_typeProfil, id_profil, id_nutriment, PU.valeur, CN.valeur, seuil_vert, seuil_orange " +
                    "   from profil_utilisateur PU, compo_nutriment CN " +
                    "  where id_produit = '" + p + "' AND  ID_critere = id_nutriment AND PU.ID_typeProfil=" + IdTpN + " ;";
                SqliteCommand command_recherche_nutriment = new SqliteCommand(sql_recherche_nutriment, connexion);
                SqliteDataReader result_recherche_nutriment = command_recherche_nutriment.ExecuteReader();
                while (result_recherche_nutriment.Read())
                {
                    String id_typeProfil = result_recherche_nutriment.GetString(0);
                    String id_profil = result_recherche_nutriment.GetString(1);
                    String id_nutriment = result_recherche_nutriment.GetString(2);
                    //decimal valeur_profil = 10;
                    decimal valeur_profil = result_recherche_nutriment.GetDecimal(3);
                    //String ValInter = result_recherche_nutriment.GetString(4).Replace('.', ',');
                    // decimal valeur_produit = decimal.Parse(ValInter);
                    decimal valeur_produit = result_recherche_nutriment.GetDecimal(4);

                    //decimal valeur_produit = 10;
                     decimal vert = result_recherche_nutriment.GetDecimal(5);
                    //decimal vert = 10;

                    //  decimal vert = result_recherche_nutriment.GetDecimal(5);
                    decimal orange = result_recherche_nutriment.GetDecimal(6);
                    //decimal orange = 10;

                    /*
                    decimal valeur_profil = result_recherche_nutriment.GetDecimal(3);
                    decimal valeur_produit = decimal.Parse(result_recherche_nutriment.GetString(4));
                    decimal vert = result_recherche_nutriment.GetDecimal(5);
                    decimal orange = result_recherche_nutriment.GetDecimal(6);
                    decimal rouge = result_recherche_nutriment.GetDecimal(7);
                    */

                    produits.AddCheckNutriment(id_typeProfil, id_profil, id_nutriment, valeur_profil, valeur_produit, vert, orange);
                }
                //Fin recherche d'allergene
                result_recherche_nutriment.Close();
                /* FIN MATCH NUTRIMENT */



                //on retourne le produit en entier
                return produits;
            }
            //Retourne le message d'erreur SQL
            catch (SqliteException ex)
            {
                String t = ex.Message;
                //pas de résultat, on va donc créer un produit vide qui renvoie l'information "aucun produit"
                Produits produits = new Produits();
                produits.SetProduits("000", "erreur", "erreur");
                return produits;
            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil


        //Scann d'un produit --> Ajout dans l'historique
        public string InsertIntoHistorique(String id_typeProfil, String id_produit)
        {
            //On tente d'abbord d'insérer un enregistrement
            try
            {
                this.ConnexionOpen();
                SqliteCommand commandDelete = connexion.CreateCommand();
                commandDelete.CommandText = "DELETE FROM historique where id_produit='" + id_produit + "';";
                commandDelete.ExecuteNonQuery();

                SqliteCommand command = connexion.CreateCommand();
                command.CommandText = "insert into historique ( id_produit, date) values ( '" + id_produit + "', datetime() );";
                command.ExecuteNonQuery();
                connexion.Close();
                return "Ok insert ";
            }
            //Si le couple id_produit et id_typeProfil existe  = on le modifie
            catch
            {
                try
                {
                    this.ConnexionOpen();
                    SqliteCommand command = connexion.CreateCommand();
                    command.CommandText = "update historique set  date = datetime() where id_typeProfil='" + id_typeProfil + "' and id_produit='" + id_produit + "' ";

                    command.ExecuteNonQuery();
                    connexion.Close();
                    return "update ";
                }
                catch
                {
                    return "Erreur update ";
                }

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }// fin CreerTableProfil




        //chargement de l'historique
        public List<Historiques> SelectHistorique()
        {
            Historiques historiques = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sql = "select id_produit, date from historique order by date desc; ";
                SqliteCommand commanda = new SqliteCommand(sql, connexion);
                SqliteDataReader result = commanda.ExecuteReader();
                List<Historiques> Hist = new List<Historiques>();

                while (result.Read())
                {
                    //Historiques historiques = new Historiques();

                    historiques = new Historiques();
                    //historiques.CreeHistorique(Int32.Parse(result.GetString(0)), result.GetString(1)) ;
                    String chaine = result.GetString(0);
                    String sqlNomProduit = "select p.product_name from produit p where p.id_produit = '" + chaine + "'; ";
                    SqliteCommand commandaNomProduit = new SqliteCommand(sqlNomProduit, connexion);
                    SqliteDataReader resultNomProduit = commandaNomProduit.ExecuteReader();
                    resultNomProduit.Read();
                    long num = long.Parse(chaine);
                    historiques.CreeHistorique(chaine, resultNomProduit.GetString(0), result.GetString(1));
                    //historiques.CreeHistorique(1, "test");
                    resultNomProduit.Close();
                    Hist.Add(historiques);
                }


                // historiques.id_produit(result.GetString(0).ToString(), result.GetString(1).ToString(), result.GetString(2).ToString());
                result.Close();
                return Hist;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<Historiques> Hist = new List<Historiques>();
                return Hist;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        //Créer la requete pour les produits recommandé

        //chargement du produit

        public String SelectLibFamille(String id)
        {
            String lib = "";

            this.ConnexionOpen();
            //Selection de l'historique
            string sql  = "select F1.ID_famille from famille_produit_main_categorie F1 where F1.ID_produit = '" + id + "'; ";
            SqliteCommand command = new SqliteCommand(sql, connexion);
            SqliteDataReader result = command.ExecuteReader();

            result.Read();

            lib = result.GetString(0);
            this.ConnexionClose();
            return lib;
        }

        public List<ProfilUtilisateurs> SelectValReco()
        {
            ProfilUtilisateurs profilUtilisateurs = null;
            try
            {
                this.ConnexionOpen();
                string sql = "select ID_critere, valeur from profil_utilisateur; ";
                SqliteCommand commanda = new SqliteCommand(sql, connexion);
                SqliteDataReader result = commanda.ExecuteReader();
                List<ProfilUtilisateurs> Prof = new List<ProfilUtilisateurs>();
                while (result.Read())
                {
                    profilUtilisateurs = new ProfilUtilisateurs();
                    String idcritere = result.GetString(0);
                    String idvaleur;
                    try { idvaleur = result.GetDecimal(1).ToString(); } catch { idvaleur = ""; }
                    profilUtilisateurs.CreeProfil(idcritere, idvaleur);
                    Prof.Add(profilUtilisateurs);
                }
                result.Close();
                return Prof;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<ProfilUtilisateurs> Prof = new List<ProfilUtilisateurs>();
                return Prof;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public List<ProduitRecos> SelectProduitRecommande(string ID)
        {
            ProduitRecos produitRecommande = new ProduitRecos();
            try
            {
                
                string sqlRecommande =
            "select F.id_famille, P.id_produit,P.product_name, C.id_nutriment, C.valeur"
            + " from compo_nutriment C, famille_produit_main_categorie F, produit P"
            + " where P.id_produit = C.id_produit"
            + " and C.id_produit = F.ID_produit"
            + " and  F.ID_famille = '" + SelectLibFamille(ID) + "'"
            + " and P.image_small_url <> '' ";


                List <ProfilUtilisateurs> ListValeurUtilisateur = new List<ProfilUtilisateurs>();

                ListValeurUtilisateur = SelectValReco();

                foreach (ProfilUtilisateurs resultat in ListValeurUtilisateur)
                {
                    // si c'est un allergene, on cherche dans la table compo allergene
                    if (this.EstUnAllergene(resultat.GetidCritere())) {
                        sqlRecommande += "  and  EXISTS  ( " +
                            " select id_produit from compo_allergene where id_produit " +
                            " NOT IN ( select id_produit from compo_allergene where id_allergene ='" + resultat.GetidCritere() + "' OR ID_allergene IN (select ID_allergene from allergene_par_cat where ID_cat_allergene in (select ID_cat_allergene from allergene_par_cat where ID_allergene = " + resultat.GetidCritere() + " ) ) ) AND id_produit = P.id_produit " +
                            "        " +
                           
                           " ) "  ;


                    }
                    //Sinon on cherche dans la table compo nutriment
                    else
                    {
                        sqlRecommande += "  and  EXISTS  ( " +
                            " select id_produit from compo_nutriment where id_produit " +
                            " NOT IN ( select id_produit from compo_nutriment where id_nutriment ='" + resultat.GetidCritere() + "' ) AND id_produit = P.id_produit" +
                            " UNION " +
                            "SELECT id_produit FROM compo_nutriment WHERE "
                    + " id_produit = P.id_produit AND id_nutriment = '" + resultat.GetidCritere() + "'"
                    + " and  valeur <= " + resultat.GetidValeur().Replace(',', '.') + ") ";
                    }
                }
                //sqlRecommande -= "or";
                //string test = "or";
                //sqlRecommande = sqlRecommande.Substring(0, sqlRecommande.Length - 2);


               sqlRecommande += "  group by P.id_produit order by P.id_produit LIMIT 40;";

                this.ConnexionOpen();
                SqliteCommand commandaReco = new SqliteCommand(sqlRecommande, connexion);
                SqliteDataReader result = commandaReco.ExecuteReader();
                List<ProduitRecos> ProduitRec = new List<ProduitRecos>();

                 while (result.Read())
                 {
                     produitRecommande = new ProduitRecos();
                     String idfamille = result.GetString(0);
                     String idproduit = result.GetString(1);
                     long num = long.Parse(idproduit);
                     String productname = result.GetString(2);
                     String idnutriment = result.GetString(3);
                    //decimal valeur = result.GetDecimal(4);
                    float val = 0;
                    try { val = result.GetFloat(4); } catch { val = 0.1f; }
                    decimal valeur = (decimal)val;

                     produitRecommande.SetProduitsReco(idfamille, idproduit,productname,idfamille,valeur);
                     ProduitRec.Add(produitRecommande);
                 }
                result.Close();
                return ProduitRec;
            }
            //Retourne le message d'erreur SQL
            catch (SQLiteException ex)
            {
                List<ProduitRecos> ProduitRec = new List<ProduitRecos>();
                String err = ex.Message;
                return ProduitRec;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public String SelectLibNutriment(String id)
        {
            String lib="";

            this.ConnexionOpen();
            //Selection de l'historique
            string sql = "select lib_nutriment from nutriment where id_nutriment = '"+id+"'; ";
            SqliteCommand command = new SqliteCommand(sql, connexion);
            SqliteDataReader result = command.ExecuteReader();

            result.Read();

            lib = result.GetString(0);
            this.ConnexionClose();
            return lib;
        }

        public List<Profils> SelectNomProfil()
        {
            Profils profilName = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlNomProfil = "select lib_profil, id_profil from profil; ";
                SqliteCommand commandaNomProfil = new SqliteCommand(sqlNomProfil, connexion);
                SqliteDataReader result = commandaNomProfil.ExecuteReader();
                List<Profils> profilNames = new List<Profils>();
                //List<string> profilListExpensible;
                while (result.Read())
                {
                    
                    //Historiques historiques = new Historiques();
                    profilName = new Profils();
                    String chaine = result.GetString(0);
                    String chaine1 = result.GetString(1);
                    profilName.CreeProfil(chaine, chaine1);
                    profilNames.Add(profilName);
                }
                result.Close();
                return profilNames;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<Profils> profilNames = new List<Profils>();
                return profilNames;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public String GetLibNutriment(String ID_Nut)
        {
            this.ConnexionOpen();
            //Selection du libellé
            string sqlLibNut = "select lib_nutriment from nutriment where id_nutriment = '"+ID_Nut+"'; ";
            SqliteCommand commandLibNut = new SqliteCommand(sqlLibNut, connexion);
            SqliteDataReader result = commandLibNut.ExecuteReader();
            result.Read();
            string rez = result.GetString(0);
            result.Close();
            this.ConnexionClose();

            return rez;
        }

        public String GetLibAllergene(String ID_All)
        {
            this.ConnexionOpen();
            //Selection du libellé
            string sqlLibAll = "select lib_allergene from allergene where ID_allergene = " + ID_All + "; ";
            SqliteCommand commandLibAll = new SqliteCommand(sqlLibAll, connexion);
            SqliteDataReader result = commandLibAll.ExecuteReader();
            result.Read();

            string rez = result.GetString(0);
            result.Close();
            this.ConnexionClose();

            return rez;
        }

        public List<ProfilsStandards> SelectcritereProfilstandard(String idc)
        {
            ProfilsStandards profilcritere = null;
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlNomProfil = "select id_profil, id_critere, valeur, seuil_vert, seuil_orange from profil_standard where id_profil = '" + idc + "' order by id_critere asc; ";
                SqliteCommand commandaNomProfilcritere = new SqliteCommand(sqlNomProfil, connexion);
                SqliteDataReader result = commandaNomProfilcritere.ExecuteReader();
                List<ProfilsStandards> profilcriteres = new List<ProfilsStandards>();
                //List<string> profilListExpensible;
                while (result.Read())
                {

                    //Historiques historiques = new Historiques();
                    profilcritere = new ProfilsStandards();
                    String chaine = result.GetString(0);
                    String chaine1 = result.GetString(1);
                    String chaine2;
                    try {  chaine2 = result.GetDecimal(2).ToString(); } catch {  chaine2 = ""; }
                    String chaine3 = result.GetDecimal(3).ToString();
                    String chaine4 = result.GetDecimal(4).ToString();
                    String chaine5;
                    try { chaine5 = result.GetDecimal(5).ToString(); } catch { chaine5 = ""; }
                    profilcritere.CreeProfilStandard(chaine, chaine1, chaine2, chaine3, chaine4, chaine5);
                    profilcriteres.Add(profilcritere);
                }
                result.Close();
                return profilcriteres;
            }
            //Retourne le message d'erreur SQL
            catch
            {
                List<ProfilsStandards> profilcriteres = new List<ProfilsStandards>();
                return profilcriteres;

            }
            //Fermeture de la connexion
            finally
            {
                this.ConnexionClose();
            }
        }

        public string VerifProfilUtilisateur(String ID, String ID_typeProfil, String ID_profil)
        {
            try
            {
                this.ConnexionOpen();
                //Selection de l'historique
                string sqlNomProfil = "select count(ps.ID_critere), pu.ID_typeProfil, pu.ID_profil, pu.valeur " +
                                        " from profil_standard ps, profil_utilisateur pu " +
                                        " where ps.ID_critere = pu.ID_critere " +
                                        " and ps.id_critere = '" + ID + "' AND pu.ID_typeProfil ='" + ID_typeProfil + "' AND pu.ID_profil = '"+ ID_profil + "'";
                SqliteCommand commandaNomProfilcritere = new SqliteCommand(sqlNomProfil, connexion);
                SqliteDataReader result = commandaNomProfilcritere.ExecuteReader();
                result.Read();
                int n = result.GetInt16(0);
                String check;
                if (n > 0) { check = "check "+result.GetString(1)+" "+ result.GetString(2)+" "+ result.GetDecimal(3); }
                else { check = "nocheck " + result.GetString(1) + " " + result.GetString(2); }
                result.Close();
                this.ConnexionClose();
                return check;
            }
            catch
            {
                this.ConnexionClose();
                return "nocheck ";
            }
           
        }

        //Récupère la Valeur d'un critère lorsqu'on appelle un couple ID_critere ID_profil
        public Decimal GetValeurProfilStansard(String ID_critere, String ID_profil) {
            try
            {
                this.ConnexionOpen();
                string sqlValeur = "SELECT valeur from profil_standard where ID_critere='" + ID_critere + "' AND ID_profil='" + ID_profil + "'   ";
                SqliteCommand commandValeur = new SqliteCommand(sqlValeur, connexion);
                SqliteDataReader result = commandValeur.ExecuteReader();
                result.Read();
                Decimal valeur = result.GetDecimal(0);
                //valeur = valeur.Replace(',', '.');
                return  valeur;

            }

            catch     {  return 0;  }
            finally   {  this.ConnexionClose();   }
        }

        //Récupère la Valeur d'un critère lorsqu'on appelle un couple ID_critere ID_profil
        public Decimal GetValeurProfilUtilisateur(String ID_critere, String ID_typeprofil, String ID_profil)
        {
            try
            {
                this.ConnexionOpen();
                string sqlValeur = "SELECT valeur from profil_utilisateur where ID_critere='" + ID_critere + "' AND ID_profil='" + ID_profil + "' AND ID_typeprofil='" + ID_typeprofil + "'     ";
                SqliteCommand commandValeur = new SqliteCommand(sqlValeur, connexion);
                SqliteDataReader result = commandValeur.ExecuteReader();
                result.Read();
               Decimal valeur = result.GetDecimal(0);
               
                return valeur;
            }

            catch { return 0; }
            finally { this.ConnexionClose(); }
        }





        //Update la valeur lorsqu'on clique sur + ou -
        public String UpdateValeur(String ID, String ID_typeProfil, String ID_profil, String val)
        {

            try
            {
                this.ConnexionOpen();
                SqliteCommand commandUpdate = connexion.CreateCommand();
                commandUpdate.CommandText = "UPDATE profil_utilisateur" +
                                            " SET valeur = " + val.Replace(',','.') + "   " +
                                            "WHERE ID_typeProfil= '" + ID_typeProfil + "' " +
                                            "AND ID_profil= '" + ID_profil + "' " +
                                            "AND ID_critere= '" + ID + "' ";
                commandUpdate.ExecuteNonQuery();
                this.ConnexionClose();
                return "ok";
            }
            catch (SqliteException ex)
            {
                return ex.Message;
            }

        }

        //Verifie si un critère passé en paramètre est un allergene, si oui on renvoi vrai
        public bool EstUnAllergene(String ID_critere)
        {
            try
            {
                this.ConnexionOpen();
                string sqlValeur = "SELECT count(ID_allergene) from allergene where ID_allergene=" + ID_critere + "   ";
                SqliteCommand commandValeur = new SqliteCommand(sqlValeur, connexion);
                SqliteDataReader result = commandValeur.ExecuteReader();
                result.Read();

                if(result.GetDecimal(0) > 0) { return true; }
                else { return false; }
                
            }

            catch { return false; }

            finally { this.ConnexionClose(); }   
        }



        //insertion dans profil_utilisateur
        public String InsertProfilUtilisateur(String ID, String ID_typeProfil, String ID_profil, String valeur )
        {
            String commande;
           

            
                if(valeur =="Vide") { valeur = "0"; }

                valeur = valeur.Replace(',', '.');

                String req = GetSeuilFromProfilStandard(ID, ID_profil);
                String[] words = req.Split(' ');
                String SEUIL_VERT = words[0];
                String SEUIL_ORANGE = words[1];

            commande = "Insert into profil_utilisateur (ID_typeProfil, ID_profil, ID_critere, valeur, SEUIL_VERT, SEUIL_ORANGE) " +
                                  "values ('" + ID_typeProfil + "', '" + ID_profil + "', '" + ID + "', " + valeur + ", " + SEUIL_VERT.Replace(',', '.') + ", " + SEUIL_ORANGE.Replace(',', '.') + ");";

            this.ConnexionOpen();
        try
            {
                
                SqliteCommand commandInsert = connexion.CreateCommand();
                commandInsert.CommandText = commande;
                commandInsert.ExecuteNonQuery();

                return "ok";

            }
            catch (SqliteException ex)
            {
                return ex.Message + " "+ commande;
            }
            //Fermeture de la connexion
           finally {

                this.ConnexionClose();
            }
            /*
            result.Close();
            this.ConnexionClose();
            return ID+" "+ ID_typeProfil;*/

        }

        public void DeleteProfilUtilisateur(String ID_typeProfil, String ID_profil, String ID_critere)
        {
            try
            {
                this.ConnexionOpen();
                SqliteCommand commandInsert = connexion.CreateCommand();
                commandInsert.CommandText = "Delete from profil_utilisateur " +
                                            " where ID_typeProfil = '" + ID_typeProfil + "' " +
                                            " AND ID_profil ='" + ID_profil + "' AND ID_critere = '" + ID_critere + "'  ";
                commandInsert.ExecuteNonQuery();
            }
            catch { }
            finally
            {
                this.ConnexionClose();

            }
            
           
        }


        public String GetNomCritereFromIdCritere(String ID_critere)
        {
            this.ConnexionOpen();
            String requeteAllergene = "SELECT a.lib_allergene " +
                            "FROM  allergene a  "+
                            "WHERE a.id_allergene = '"+ ID_critere + "'; ";
            String requeteNutriment = "SELECT n.lib_nutriment " +
                            " FROM  nutriment n  " +
                            " WHERE LOWER(n.id_nutriment) = LOWER('" + ID_critere + "'); ";

            SqliteCommand command = new SqliteCommand(requeteAllergene, connexion);
            SqliteDataReader result = command.ExecuteReader();
            result.Read();
            try { return result.GetString(0); }
            catch {
                SqliteCommand commandNut = new SqliteCommand(requeteNutriment, connexion);
                SqliteDataReader resultNut = commandNut.ExecuteReader();
                resultNut.Read();
                return resultNut.GetString(0);

            }
            finally
            {
                this.ConnexionClose();
            }

        }

        public String GetSeuilFromProfilStandard(String ID_critere, String ID_profil)
        {
            this.ConnexionOpen();
            String requeteSeuil = "SELECT SEUIL_VERT, SEUIL_ORANGE FROM profil_standard "+
                                    " where ID_critere = '"+ ID_critere + "' AND ID_profil = '"+ ID_profil + "' ; ";

            SqliteCommand command = new SqliteCommand(requeteSeuil, connexion);
            SqliteDataReader result = command.ExecuteReader();
            result.Read();

            string seuil = result.GetDecimal(0) + " " + result.GetDecimal(1);

            result.Close();
            this.ConnexionClose();

            return seuil;

            }


        




    }
}