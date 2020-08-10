/*-------------------------------------------------------------------------------------------
 * Copyright (c) Fuyuno Mikazuki / Natsuneko. All rights reserved.
 * Licensed under the MIT License. See LICENSE in the project root for license information.
 *------------------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;
using UnityEngine.Animations;

using Object = UnityEngine.Object;

namespace Assets.Mochizuki.ConstraintByHumanoid
{
    public class ConstraintEditor : EditorWindow
    {
        private const string Product = "Constraint by Humanoid";
        private const string Version = "0.1.0";
        private readonly GUIContent[] _items;

        private Constraint _constraint;
        private GameObject _dst;

        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        [SerializeField]
        private List<string> _errors;

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

        [MenuItem("Mochizuki/Unity/Constraint by Humanoid")]
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
                EditorGUILayout.LabelField("Unity の Humanoid Rig の規約に沿っているアバター同士の Constraint の設定を自動で行うエディター拡張です。");

            EditorGUI.BeginChangeCheck();

            _src = ObjectPicker("Source GameObject", _src);
            _dst = ObjectPicker("Destination GameObject", _dst);

            _constraint = (Constraint) EditorGUILayout.Popup(new GUIContent("Constraint"), (int) _constraint, _items);

            if (EditorGUI.EndChangeCheck())
            {
                _errors.Clear();
                if (_src == null)
                    _errors.Add("Source GameObject を設定してください。");
                else
                    _errors.AddRange(CheckGameObject(_src));

                if (_dst == null)
                    _errors.Add("Destination GameObject を設定してください。");
                else
                    _errors.AddRange(CheckGameObject(_dst));

                if (_src == _dst)
                    _errors.Add("Source と Destination に同じ GameObject は設定できません。");
            }

            if (_errors.Count > 0)
                foreach (var error in _errors.Distinct())
                    EditorGUILayout.HelpBox(error, MessageType.Error);

            EditorGUI.BeginDisabledGroup(_errors.Count > 0);

            if (GUILayout.Button("変更を適用"))
                ApplyChanges(_src, _dst, _constraint);

            EditorGUI.EndDisabledGroup();
        }

        private static IEnumerable<string> CheckGameObject(GameObject gameObject)
        {
            var errors = new List<string>();
            if (gameObject.GetComponent<Animator>() == null)
                errors.Add($"`{gameObject.name}` は Animator Component を所持している必要があります。");
            if (gameObject.GetComponentsInChildren<Transform>().All(w => w.name != "Armature"))
                errors.Add($"`{gameObject.name}` は Armature GameObject を子に所持している必要があります。");

            return errors;
        }

        private static void ApplyChanges(GameObject src, GameObject dst, Constraint constraint)
        {
            ConfigureConstraint(src, dst, constraint);
        }

        private static void ConfigureConstraint(GameObject src, GameObject dst, Constraint constraint)
        {
            var s = src.GetComponent<Animator>();
            var d = dst.GetComponent<Animator>();

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.Hips);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.Spine);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.Chest);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.UpperChest);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftShoulder);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftUpperArm);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftLowerArm);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftHand);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightShoulder);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightUpperArm);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightLowerArm);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightHand);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftUpperLeg);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftLowerLeg);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftFoot);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftToes);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightUpperLeg);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightLowerLeg);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightFoot);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightToes);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.Neck);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.Head);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftEye);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightEye);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftThumbProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftThumbIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftThumbDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftIndexProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftIndexIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftIndexDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftMiddleProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftMiddleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftMiddleDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftRingProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftRingIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftRingDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftLittleProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftLittleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.LeftLittleDistal);

            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightThumbProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightThumbIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightThumbDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightIndexProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightIndexIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightIndexDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightMiddleProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightMiddleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightMiddleDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightRingProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightRingIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightRingDistal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightLittleProximal);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightLittleIntermediate);
            ConfigureRotationConstraintToGameObject(s, d, constraint, HumanBodyBones.RightLittleDistal);
        }

        private static void ConfigureRotationConstraintToGameObject(Animator src, Animator dst, Constraint type, HumanBodyBones id)
        {
            var srcBone = src.GetBoneTransform(id);
            var dstBone = dst.GetBoneTransform(id);

            if (srcBone == null || dstBone == null)
                return;

            var srcGameObject = srcBone.gameObject;
            var dstGameObject = dstBone.gameObject;

            var constraint = AddConstraintToGameObject(dstGameObject, type);
            var source = new ConstraintSource { sourceTransform = srcGameObject.transform, weight = 1.0f };
            constraint.AddSource(source);
            constraint.constraintActive = true;
        }

        private static IConstraint AddConstraintToGameObject(GameObject obj, Constraint constraint)
        {
            switch (constraint)
            {
                case Constraint.AimConstraint:
                    return obj.AddComponent<AimConstraint>();

                case Constraint.LookAtConstraint:
                    return obj.AddComponent<LookAtConstraint>();

                case Constraint.ParentConstraint:
                    return obj.AddComponent<ParentConstraint>();

                case Constraint.PositionConstraint:
                    return obj.AddComponent<PositionConstraint>();

                case Constraint.RotationConstraint:
                    return obj.AddComponent<RotationConstraint>();

                case Constraint.ScaleConstraint:
                    return obj.AddComponent<ScaleConstraint>();

                default:
                    throw new ArgumentOutOfRangeException(nameof(constraint), constraint, null);
            }
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