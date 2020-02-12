using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Models;

namespace TodoListAPI.Data
{
   /// <summary>
   /// Interface pour un contexte de données de gestion des tâches
   /// </summary>
   public interface ITodoListContext
   {
      // Renvoie la liste de tâches
      List<Tache> GetTaches();

      // Ajoute une nouvelle tâche
      void AjouterTache(Tache tache);

      // Modifie une tâche
      void ModifierTache(Tache tache);

      // Supprime la tâche dont l'id est passé en paramètre
      void SupprimerTache(int id);
   }
}
