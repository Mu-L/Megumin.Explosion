﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.Reflection;
using UnityEditor.ShortcutManagement;
using System;

namespace Megumin
{
    [InitializeOnLoad]
    public class DebugableCallback
    {
        static bool Enable = false;

        [InitializeOnLoadMethod]
        public static void Register()
        {
            //Debug.Log("注册");


            //#if ENABLE_INPUT_SYSTEM
            //            InputSystem.onEvent -= InputSystem_onEvent;
            //            InputSystem.onEvent += InputSystem_onEvent;
            //#endif

            Enable = true;
        }

        private unsafe static void InputSystem_onEvent(UnityEngine.InputSystem.LowLevel.InputEventPtr arg1,
                                            InputDevice arg2)
        {
            if (Selection.activeGameObject)
            {
                if (arg2 is Keyboard keyboard)
                {
                    var res = keyboard.ReadValueFromEventAsObject(arg1);
                    var res2 = keyboard.f1Key.ReadValueFromEventAsObject(arg1);
                    //if (keyboard.f1Key.wasPressedThisFrame)
                    if (res2 is float a && a == 1)
                    {
                        OnF1();
                    }
                }
            }
        }

        public static void ObjectOnF1(object item)
        {
            if (item is IF1able f1Able)
            {
                f1Able?.OnF1();
            }

            {
                //控制台Log字段
                var fields = from f in item.GetType().GetFields((BindingFlags)(-1))
                             let attrs = f.GetCustomAttributes(typeof(OnF1Attribute), true)
                             where attrs.Length > 0
                             select f;
                foreach (var field in fields)
                {
                    var fv = field.GetValue(item);
                    Debug.Log(fv.ToStringReflection(true));
                }
            }

            {
                //控制台Log属性
                var props = from p in item.GetType().GetProperties((BindingFlags)(-1))
                            let attrs = p.GetCustomAttributes(typeof(OnF1Attribute), true)
                            where attrs.Length > 0
                            select p;
                foreach (var property in props)
                {
                    var pv = property.GetValue(item);
                    Debug.Log(pv.ToStringReflection(true));
                }
            }

            {
                //反射调用方法
                var methods = from m in item.GetType().GetMethods((BindingFlags)(-1))
                              let attrs = m.GetCustomAttributes(typeof(OnF1Attribute), true)
                              where attrs.Length > 0
                              select m;

                foreach (var method in methods)
                {
                    method.Invoke(item, null);
                }
            }
        }

        public static void ObjectOnKey(object item, ConsoleKey key)
        {
            {
                //控制台Log字段
                var fields = from f in item.GetType().GetFields((BindingFlags)(-1))
                             let attrs = f.GetCustomAttributes(typeof(OnKeyAttribute), true)
                             where attrs.Length > 0
                             select (f, attrs);

                foreach (var kv in fields)
                {
                    if (kv.attrs.Any(a => a is OnKeyAttribute ok && ok.Key == key))
                    {
                        var fv = kv.f.GetValue(item);
                        Debug.Log(fv.ToStringReflection(true));
                    }
                }
            }

            {
                //控制台Log属性
                var props = from p in item.GetType().GetProperties((BindingFlags)(-1))
                            let attrs = p.GetCustomAttributes(typeof(OnKeyAttribute), true)
                            where attrs.Length > 0
                            select (p, attrs);

                foreach (var kv in props)
                {
                    if (kv.attrs.Any(a => a is OnKeyAttribute ok && ok.Key == key))
                    {
                        var fv = kv.p.GetValue(item);
                        Debug.Log(fv.ToStringReflection(true));
                    }
                }
            }

            {
                //反射调用方法
                var methods = from m in item.GetType().GetMethods((BindingFlags)(-1))
                              let attrs = m.GetCustomAttributes(typeof(OnKeyAttribute), true)
                              where attrs.Length > 0
                              select (m, attrs);

                foreach (var kv in methods)
                {
                    if (kv.attrs.Any(a => a is OnKeyAttribute ok && ok.Key == key))
                    {
                        var method = kv.m;
                        method.Invoke(item, null);
                    }
                }
            }
        }

        public static void OnKey(ConsoleKey key)
        {
            if (Selection.activeGameObject)
            {
                var comps = Selection.activeGameObject.GetComponents<MonoBehaviour>();
                foreach (var item in comps)
                {
                    ObjectOnKey(item, key);
                }
            }

            if (Selection.activeObject is ScriptableObject obj)
            {
                ObjectOnKey(obj, key);
            }
        }

        [Shortcut("MeguminDebug/" + nameof(OnF1), KeyCode.F1)]
        public static void OnF1()
        {
            if (Selection.activeGameObject)
            {
                var comps = Selection.activeGameObject.GetComponents<MonoBehaviour>();
                foreach (var item in comps)
                {
                    ObjectOnF1(item);
                }
            }

            if (Selection.activeObject is ScriptableObject obj)
            {
                ObjectOnF1(obj);
            }

            OnKey(ConsoleKey.F1);
        }

        [Shortcut("MeguminDebug/" + nameof(OnF3), KeyCode.F3)]
        public static void OnF3()
        {
            OnKey(ConsoleKey.F3);
        }

        [Shortcut("MeguminDebug/" + nameof(OnF4), KeyCode.F4)]
        public static void OnF4()
        {
            OnKey(ConsoleKey.F4);
        }

    }

}