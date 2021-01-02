using System;
using System.Collections.Generic;

using MLAPI.Connection;
using UnityEngine;

namespace MLAPI.AOI
{
   public class _ClientObjectMapBaseNode<CLIENT, OBJECT>
   {
       // set this delegate if you want a function called when
       public delegate void TeardownHandler(OBJECT obj);
       protected TeardownHandler OnDespawn;

       public _ClientObjectMapBaseNode() : base()
       {
           children = new List<_ClientObjectMapBaseNode<CLIENT, OBJECT>>();
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

        public void AddNode(_ClientObjectMapBaseNode<CLIENT, OBJECT> newNode)
        {
            children.Add(newNode);
        }

        private List<_ClientObjectMapBaseNode<CLIENT, OBJECT>> children;
   }

    public class _ClientObjectMapDynamicNode<CLIENT, OBJECT> : _ClientObjectMapBaseNode<CLIENT, OBJECT>
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


    public class _ClientObjectMapStaticNode<CLIENT, OBJECT> : _ClientObjectMapBaseNode<CLIENT, OBJECT>
    {
        public _ClientObjectMapStaticNode() : base()
        {
            alwaysRelevant = new HashSet<OBJECT>();
            OnDespawn = delegate(OBJECT o)
            {
                alwaysRelevant.Remove(o);
            };
        }

        public void AddStatic(OBJECT o) //??
        {
            alwaysRelevant.Add(o);
        }

        public override void OnQuery(CLIENT client, HashSet<OBJECT> results)
        {
            results.UnionWith(alwaysRelevant);
        }

        private HashSet<OBJECT> alwaysRelevant;
    }

    public class ClientObjectMapStaticNode : _ClientObjectMapStaticNode<NetworkedClient, NetworkedObject>
    {
    }

    public class ClientObjectMapDynamicNode : _ClientObjectMapDynamicNode<NetworkedClient, NetworkedObject>
    {
    }
}
