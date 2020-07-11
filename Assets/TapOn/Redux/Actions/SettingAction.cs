using System.Collections;
using System.Collections.Generic;
using TapOn.Models.DataModels;
using UnityEngine;

namespace TapOn.Redux.Actions
{
    public class ChangeIndexAction
    {
        public int index;
    }

    public class AddProductAction
    {
        public Product product;
    }
}
