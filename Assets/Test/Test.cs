using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class ListNode
{
     public int val;
     public ListNode next;
     public ListNode(int val = 0, ListNode next = null)
     {
         this.val = val;
          this.next = next;
     }
}
 
public class Test : MonoBehaviour
{
    public ListNode ReverseList(ListNode head)
    {
        List<ListNode> listNodes = new List<ListNode>();
        ListNode listNode= head;
        while (listNode != null)
        {
            listNodes.Add(listNode);
            listNode = listNode.next;
        }
        return listNodes[listNodes.Count-1];
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
