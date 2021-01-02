using System;
using System.Collections.Generic;

using MLAPI.Connection;
using UnityEngine;

namespace MLAPI.AOI
{
   public class ClientObjMapNodeBase<CLIENT, OBJECT>
   {
       // set this delegate if you want a function called when
       public delegate void TeardownHandler(OBJECT obj);
       protected TeardownHandler OnDespawn;

       public ClientObjMapNodeBase() : base()
       {
           children = new List<ClientObjMapNodeBase<CLIENT, OBJECT>>();
       }

        public virtual void OnQuery(CLIENT client, HashSet<OBJECT> results) { }

        public HashSet<OBJECT> QueryFor(CLIENT client)
        {
            HashSet<OBJECT> results = new HashSet<OBJECT>();
            DoQueryFor(client, results);
            return results;
        }

       private void DoQueryFor(CLIENT client, HashSet<OBJECT> results)
        {
            OnQuery(client, results);

            foreach (var c in children)
            {
                c.DoQueryFor(client, results);
            }
        }

        public void DespawnCleanup(OBJECT o)
        {
            if (OnDespawn != null)
            {
                OnDespawn(o); // RENAME to OnDespawn
            }

            foreach (var c in children)
            {
                c.DespawnCleanup(o);
            }
        }

        public void AddNode(ClientObjMapNodeBase<CLIENT, OBJECT> @new)
        {
            children.Add(@new);
        }

        private List<ClientObjMapNodeBase<CLIENT, OBJECT>> children;
   }

    public class ClientObjMapNodeDynamic<CLIENT, OBJECT> : ClientObjMapNodeBase<CLIENT, OBJECT>
    {
        public delegate void DynamicQuery(CLIENT client, HashSet<OBJECT> results);
        protected DynamicQuery dynamicQuery;

        public override void OnQuery(CLIENT client, HashSet<OBJECT> results)
        {
            if (dynamicQuery != null)
            {
                dynamicQuery(client, results);
            }
        }
    }

    public class ClientObjMapNodeStatic<CLIENT, OBJECT> : ClientObjMapNodeBase<CLIENT, OBJECT>
    {
        public ClientObjMapNodeStatic() : base()
        {
            alwaysRelevant = new HashSet<OBJECT>();

            // when we are told an object is despawning, remove it from our list
            OnDespawn = delegate(OBJECT o)
            {
                alwaysRelevant.Remove(o);
            };
        }

        // Add a new item to our static list
        public void Add(OBJECT o)
        {
            alwaysRelevant.Add(o);
        }

        // for our query, we simply union our static objects with the results
        //  more sophisticated methods might be explored later, like having the results
        //  list contain not ust
        public override void OnQuery(CLIENT client, HashSet<OBJECT> results)
        {
            results.UnionWith(alwaysRelevant);
        }

        private HashSet<OBJECT> alwaysRelevant;
    }
}
