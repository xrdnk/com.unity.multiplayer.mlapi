using System;
using System.Collections.Generic;

using MLAPI.Connection;
using UnityEngine;

namespace MLAPI.AOI
{
   public class _ClientObjectMap<CLIENT, OBJECT>
   {
      private _ClientObjectMapBaseNode<CLIENT, OBJECT> root;

      public _ClientObjectMap() : base()
      {
          Debug.Log("IM XTOR");
          root = new _ClientObjectMapBaseNode<CLIENT, OBJECT>();
      }

      public HashSet<OBJECT> QueryFor(CLIENT client)
      {
          HashSet<OBJECT> results = new HashSet<OBJECT>();
          root.QueryFor(client, results);
          return results;
      }

      public void DespawnCleanup(OBJECT no)
      {
          root.DespawnCleanup(no);
      }

      public void AddNode(_ClientObjectMapBaseNode<CLIENT, OBJECT> n)
      {
          root.AddNode(n);
      }

      private Dictionary<string, _ClientObjectMapBaseNode<CLIENT, OBJECT>> nodeDictionary;
   }

   public class _ClientObjectMapBaseNode<CLIENT, OBJECT>
   {
       public delegate void TeardownHandler(OBJECT obj);
       protected TeardownHandler OnTeardown;

       public _ClientObjectMapBaseNode() : base()
       {
           children = new List<_ClientObjectMapBaseNode<CLIENT, OBJECT>>();
       }

       public virtual void OnDespawn(OBJECT o) { }

        public virtual void OnQuery(CLIENT client, HashSet<OBJECT> results) { }

       public void QueryFor(CLIENT client, HashSet<OBJECT> results)
        {
            OnQuery(client, results);

            foreach (var c in children)
            {
                c.QueryFor(client, results);
            }
        }


        public void DespawnCleanup(OBJECT o)
        {
            if (OnTeardown != null)
            {
                OnTeardown(o); // ??
            }

            OnDespawn(o);

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

        public _ClientObjectMapDynamicNode()
        {
            Debug.Log("IN DYN XTOR");
        }

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
        }

        public void AddStatic(OBJECT o) //??
        {
            alwaysRelevant.Add(o);
        }

        public override void OnDespawn(OBJECT o)
        {
            alwaysRelevant.Remove(o);
        }

        public override void OnQuery(CLIENT client, HashSet<OBJECT> results)
        {
            results.UnionWith(alwaysRelevant);
        }

        private HashSet<OBJECT> alwaysRelevant;
    }



    public class ClientObjectMap : _ClientObjectMap<NetworkedClient, NetworkedObject>
    {
    }

    public class ClientObjectMapStaticNode : _ClientObjectMapStaticNode<NetworkedClient, NetworkedObject>
    {
    }

    public class ClientObjectMapDynamicNode : _ClientObjectMapDynamicNode<NetworkedClient, NetworkedObject>
    {
    }
}
