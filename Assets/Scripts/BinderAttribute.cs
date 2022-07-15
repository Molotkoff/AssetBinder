using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

namespace AMolotkoff.Binder
{
    [AttributeUsage(AttributeTargets.Field)]
    public class BinderAttribute : Attribute {}
}