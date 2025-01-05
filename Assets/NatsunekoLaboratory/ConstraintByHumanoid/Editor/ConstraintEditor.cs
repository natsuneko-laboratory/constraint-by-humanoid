// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using VRC.Dynamics;
using VRC.SDK3.Dynamics.Constraint.Components;
using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.ConstraintByHumanoid
{
    public class ConstraintEditor : EditorWindow
    {
        private const string Product = "Constraint by Humanoid";
        private const string Version = "0.4.0";
        private readonly GUIContent[] _items;

        private Constraint _constraint;
        private GameObject _dst;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField]
        private List<string> _errors;

        [SerializeField]
        private GameObject[] _excludes;

        private bool _isShowExcludeSettings;
        private Vector2 _scroll;

        private GameObject _src;

        public ConstraintEditor()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (_errors == null)
                _errors = new List<string>();
            _items = Enum.GetNames(typeof(Constraint))
                         .Select(w => Regex.Replace(w, "(\\B[A-Z])", " $1"))
                         .Select(w => new GUIContent(w))
                         .ToArray();
        }

        [MenuItem("Window/NatsunekoLaboratory/Constraint by Humanoid")]
        public static void ShowWindow()
        {
            var window = GetWindow<ConstraintEditor>();
            window.titleContent = new GUIContent("Constraint Editor");

            window.Show();
        }

        // ReSharper disable once UnusedMember.Local
        private void OnGUI()
        {
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"{Product} - {Version}");
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                EditorGUILayout.LabelField("The Unity editor extension that automatically set-up constraints between two rigs based on Unity Humanoid Rig Standard.");

            EditorGUI.BeginChangeCheck();

            using (var scroller = new EditorGUILayout.ScrollViewScope(_scroll, GUILayout.ExpandHeight(true)))
            {
                _scroll = scroller.scrollPosition;

                _src = ObjectPicker("Source GameObject", _src);
                _dst = ObjectPicker("Destination GameObject", _dst);

                _isShowExcludeSettings = EditorGUILayout.Foldout(_isShowExcludeSettings, "Exclude GameObjects");
                if (_isShowExcludeSettings)
                    using (new EditorGUI.IndentLevelScope())
                    {
                        PropertyField(this, nameof(_excludes));

                        var area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
                        GUI.Box(area, "Drag and Drop GameObjects that you want to exclude");

                        if (area.Contains(Event.current.mousePosition))
                            switch (Event.current.type)
                            {
                                case EventType.DragUpdated:
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                                    Event.current.Use();
                                    break;

                                case EventType.DragPerform:
                                    DragAndDrop.AcceptDrag();

                                    var references = DragAndDrop.objectReferences;
                                    _excludes = (_excludes ?? Array.Empty<GameObject>()).Concat(references).Distinct().Cast<GameObject>().ToArray();

                                    DragAndDrop.activeControlID = 0;
                                    Event.current.Use();
                                    break;
                            }
                    }

                _constraint = (Constraint)EditorGUILayout.Popup(new GUIContent("Constraint"), (int)_constraint, _items);
            }

            if (EditorGUI.EndChangeCheck())
            {
                _errors.Clear();
                if (_src == null)
                    _errors.Add("Set the Source GameObject");
                else
                    _errors.AddRange(CheckGameObject(_src));

                if (_dst == null)
                    _errors.Add("Set the Destination GameObject");
                else
                    _errors.AddRange(CheckGameObject(_dst));

                if (_src == _dst)
                    _errors.Add("Could not set the same GameObject as the Source and Destination");
            }

            if (_errors.Count > 0)
                foreach (var error in _errors.Distinct())
                    EditorGUILayout.HelpBox(error, MessageType.Error);

            EditorGUI.BeginDisabledGroup(_errors.Count > 0);

            if (GUILayout.Button("Apply Changes"))
                ApplyChanges(_src, _dst, _excludes ?? Array.Empty<GameObject>(), _constraint);

            EditorGUI.EndDisabledGroup();
        }

        private static IEnumerable<string> CheckGameObject(GameObject gameObject)
        {
            var errors = new List<string>();
            if (gameObject.GetComponent<Animator>() == null)
                errors.Add($"`{gameObject.name}` must have an Animator Component");
            if (gameObject.GetComponentsInChildren<Transform>().All(w => w.name.ToLower() != "armature"))
                errors.Add($"`{gameObject.name}` must have an Armature GameObject as a child");

            return errors;
        }

        private static void ApplyChanges(GameObject src, GameObject dst, GameObject[] excludes, Constraint constraint)
        {
            ConfigureConstraint(src, dst, excludes, constraint);
        }

        private static void ConfigureConstraint(GameObject src, GameObject dst, GameObject[] excludes, Constraint constraint)
        {
            var s = src.GetComponent<Animator>();
            var d = dst.GetComponent<Animator>();

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.Hips);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.Spine);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.Chest);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.UpperChest);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftShoulder);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftUpperArm);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftLowerArm);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftHand);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightShoulder);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightUpperArm);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightLowerArm);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightHand);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftUpperLeg);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftLowerLeg);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftFoot);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftToes);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightUpperLeg);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightLowerLeg);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightFoot);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightToes);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.Neck);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.Head);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftEye);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightEye);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftThumbProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftThumbIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftThumbDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftIndexProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftIndexIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftIndexDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftMiddleProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftMiddleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftMiddleDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftRingProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftRingIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftRingDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftLittleProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftLittleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.LeftLittleDistal);

            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightThumbProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightThumbIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightThumbDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightIndexProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightIndexIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightIndexDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightMiddleProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightMiddleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightMiddleDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightRingProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightRingIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightRingDistal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightLittleProximal);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightLittleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, excludes, constraint, HumanBodyBones.RightLittleDistal);
        }

        private static void ConfigureRotationConstraintToGameObject(Animator src, Animator dst, GameObject[] excludes, Constraint type, HumanBodyBones id)
        {
            var srcBone = src.GetBoneTransform(id);
            var dstBone = dst.GetBoneTransform(id);

            if (srcBone == null || dstBone == null)
                return;

            var srcGameObject = srcBone.gameObject;
            var dstGameObject = dstBone.gameObject;

            if (excludes.Contains(srcGameObject) || excludes.Contains(dstGameObject))
                return;

            if (dstGameObject.GetComponent(GetTypeFromConstraint(type)) != null)
            {
                Debug.LogWarning($"The GameObject `{dstGameObject.name}` has been skipped because it already has {type.ToString()}.");
                return;
            }

            var constraint = AddConstraintToGameObject(dstGameObject, type);
            var source = new VRCConstraintSource { SourceTransform = srcGameObject.transform, Weight = 1.0f };
            constraint.Sources.Add(source);
            constraint.ActivateConstraint();
        }

        private static Type GetTypeFromConstraint(Constraint constraint)
        {
            switch (constraint)
            {
                case Constraint.AimConstraint:
                    return typeof(VRCAimConstraint);

                case Constraint.LookAtConstraint:
                    return typeof(VRCLookAtConstraint);

                case Constraint.ParentConstraint:
                    return typeof(VRCParentConstraint);

                case Constraint.PositionConstraint:
                    return typeof(VRCPositionConstraint);

                case Constraint.RotationConstraint:
                    return typeof(VRCRotationConstraint);

                case Constraint.ScaleConstraint:
                    return typeof(VRCScaleConstraint);

                default:
                    throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
            }
        }

        private static VRCConstraintBase AddConstraintToGameObject(GameObject obj, Constraint constraint)
        {
            switch (constraint)
            {
                case Constraint.AimConstraint:
                    return obj.AddComponent<VRCAimConstraint>();

                case Constraint.LookAtConstraint:
                    return obj.AddComponent<VRCLookAtConstraint>();

                case Constraint.ParentConstraint:
                    return obj.AddComponent<VRCParentConstraint>();

                case Constraint.PositionConstraint:
                    return obj.AddComponent<VRCPositionConstraint>();

                case Constraint.RotationConstraint:
                    return obj.AddComponent<VRCRotationConstraint>();

                case Constraint.ScaleConstraint:
                    return obj.AddComponent<VRCScaleConstraint>();

                default:
                    throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
            }
        }

        private static void PropertyField(EditorWindow editor, string property)
        {
            var so = new SerializedObject(editor);
            so.Update();

            EditorGUILayout.PropertyField(so.FindProperty(property), true);

            so.ApplyModifiedProperties();
        }

        private static T ObjectPicker<T>(string label, T obj) where T : Object
        {
            return EditorGUILayout.ObjectField(new GUIContent(label), obj, typeof(T), true) as T;
        }

        private enum Constraint
        {
            AimConstraint,

            LookAtConstraint,

            ParentConstraint,

            PositionConstraint,

            RotationConstraint,

            ScaleConstraint
        }
    }
}
