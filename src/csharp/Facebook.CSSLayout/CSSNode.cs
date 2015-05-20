/**
 * Copyright (c) 2014, Facebook, Inc.
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the
 * LICENSE file in the root directory of this source tree. An additional grant
 * of patent rights can be found in the PATENTS file in the same directory.
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Facebook.CSSLayout
{
	/**
	 * Should measure the given node and put the result in the given MeasureOutput.
	 *
	 * NB: measure is NOT guaranteed to be threadsafe/re-entrant safe!
	 */

	public delegate MeasureOutput MeasureFunction(CSSNode node, float width);

	/**
	 * A CSS Node. It has a style object you can manipulate at {@link #style}. After calling
	 * {@link #calculateLayout()}, {@link #layout} will be filled with the results of the layout.
	 */

	public class CSSNode
	{
		enum LayoutState
		{
			/**
		 * Some property of this node or its children has changes and the current values in
		 * {@link #layout} are not valid.
		 */
			DIRTY,

			/**
		 * This node has a new layout relative to the last time {@link #MarkLayoutSeen()} was called.
		 */
			HAS_NEW_LAYOUT,

			/**
		 * {@link #layout} is valid for the node's properties and this layout has been marked as
		 * having been seen.
		 */
			UP_TO_DATE,
		}

		internal readonly CSSStyle style = new CSSStyle();
		internal readonly CSSLayout layout = new CSSLayout();
		internal readonly CachedCSSLayout lastLayout = new CachedCSSLayout();

		internal int lineIndex = 0;

		// 4 is kinda arbitrary, but the default of 10 seems really high for an average View.
		readonly List<CSSNode> mChildren = new List<CSSNode>(4);
		[Nullable] CSSNode mParent;
		[Nullable] MeasureFunction mMeasureFunction = null;
		LayoutState mLayoutState = LayoutState.DIRTY;

		public int ChildCount { get {  return mChildren.Count;} }

		public CSSNode this[int i]
		{
			get { return mChildren[i]; }
		}

		public IEnumerable<CSSNode> Children
		{
			get { return mChildren; }
		}

		public void AddChild(CSSNode child)
		{
			InsertChild(ChildCount, child);
		}

		public void InsertChild(int i, CSSNode child)
		{
			if (child.mParent != null)
			{
				throw new InvalidOperationException("Child already has a parent, it must be removed first.");
			}

			mChildren.Insert(i, child);
			child.mParent = this;
			dirty();
		}

		public void RemoveChildAt(int i)
		{
			mChildren[i].mParent = null;
			mChildren.RemoveAt(i);
			dirty();
		}

		public void RemoveSelf()
		{
			if (mParent == null)
				return;
			var index = mParent.IndexOf(this);
			if (index == -1)
				throw new InvalidOperationException("Child's parent does not contain it.");
			mParent.RemoveChildAt(index);
		}

		public CSSNode Parent
		{
			[return: Nullable]
			get { return mParent; }
		}

		/**
	   * @return the index of the given child, or -1 if the child doesn't exist in this node.
	   */

		public int IndexOf(CSSNode child)
		{
			return mChildren.IndexOf(child);
		}

		public MeasureFunction MeasureFunction
		{
			get { return mMeasureFunction; }
			set
			{
				if (!valuesEqual(mMeasureFunction, value))
				{
					mMeasureFunction = value;
					dirty();
				}
			}
		}

		public bool IsMeasureDefined 
		{
			get { return mMeasureFunction != null; }
		}

		internal MeasureOutput measure(float width)
		{
			if (!IsMeasureDefined)
			{
				throw new Exception("Measure function isn't defined!");
			}
			return Assertions.assertNotNull(mMeasureFunction)(this, width);
		}

		/**
	   * Performs the actual layout and saves the results in {@link #layout}
	   */

		public void CalculateLayout()
		{
			layout.resetResult();
			LayoutEngine.layoutNode(this, CSSConstants.Undefined, null);
		}

		/**
	   * See {@link LayoutState#DIRTY}.
	   */

		public bool IsDirty
		{
			get
			{
				return mLayoutState == LayoutState.DIRTY;
			}
		}

		/**
	   * See {@link LayoutState#HAS_NEW_LAYOUT}.
	   */

		public bool HasNewLayout
		{
			get { return mLayoutState == LayoutState.HAS_NEW_LAYOUT; }
		}

		/*
			Additional function to mark this node as dirty without requiring a derived class, thereby undermining
			the original protection of the dirty() method. 
		
			Calling this function is only required when the measure function is the same, but changes its behavior.
			For all other property changes, the node is automatically marked dirty.
		*/

		public void MarkDirty()
		{
			dirty();
		}

		protected void dirty()
		{
			if (mLayoutState == LayoutState.DIRTY)
			{
				return;
			}
			else if (mLayoutState == LayoutState.HAS_NEW_LAYOUT)
			{
				throw new InvalidOperationException("Previous layout was ignored! MarkLayoutSeen() never called");
			}

			mLayoutState = LayoutState.DIRTY;

			if (mParent != null)
			{
				mParent.dirty();
			}
		}

		internal void markHasNewLayout()
		{
			mLayoutState = LayoutState.HAS_NEW_LAYOUT;
		}

		/**
	   * Tells the node that the current values in {@link #layout} have been seen. Subsequent calls
	   * to {@link #hasNewLayout()} will return false until this node is laid out with new parameters.
	   * You must call this each time the layout is generated if the node has a new layout.
	   */

		public void MarkLayoutSeen()
		{
			if (!HasNewLayout)
			{
				throw new InvalidOperationException("Expected node to have a new layout to be seen!");
			}

			mLayoutState = LayoutState.UP_TO_DATE;
		}

		void toStringWithIndentation(StringBuilder result, int level)
		{
			// Spaces and tabs are dropped by IntelliJ logcat integration, so rely on __ instead.
			StringBuilder indentation = new StringBuilder();
			for (int i = 0; i < level; ++i)
			{
				indentation.Append("__");
			}

			result.Append(indentation.ToString());
			result.Append(layout.ToString());

			if (ChildCount == 0)
			{
				return;
			}

			result.Append(", children: [\n");
			for (var i = 0; i < ChildCount; i++)
			{
				this[i].toStringWithIndentation(result, level + 1);
				result.Append("\n");
			}
			result.Append(indentation + "]");
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			this.toStringWithIndentation(sb, 0);
			return sb.ToString();
		}

		protected bool valuesEqual(float f1, float f2)
		{
			return FloatUtil.floatsEqual(f1, f2);
		}

		protected bool valuesEqual<T>([Nullable] T o1, [Nullable] T o2)
		{
			if (o1 == null)
			{
				return o2 == null;
			}
			return o1.Equals(o2);
		}

		public CSSFlexDirection FlexDirection
		{
			get { return style.flexDirection; }
			set { updateDiscreteValue(ref style.flexDirection, value); }
		}

		public CSSJustify JustifyContent
		{
			get { return style.justifyContent; }
			set { updateDiscreteValue(ref style.justifyContent, value); }
		}

		public CSSAlign AlignItems
		{
			get { return style.alignItems; }
			set { updateDiscreteValue(ref style.alignItems, value); }
		}

		public CSSAlign AlignSelf
		{
			get { return style.alignSelf; }
			set { updateDiscreteValue(ref style.alignSelf, value); }
		}

		public CSSPositionType PositionType
		{
			get { return style.positionType; }
			set { updateDiscreteValue(ref style.positionType, value); }
		}

		public CSSWrap Wrap
		{
			get { return style.flexWrap; }
			set { updateDiscreteValue(ref style.flexWrap, value); }
		}

		public float Flex
		{
			get { return style.flex; }
			set { updateFloatValue(ref style.flex, value); }
		}

		public void SetMargin(SpacingType spacingType, float margin)
		{
			if (style.margin.set((int)spacingType, margin))
				dirty();
		}

		internal void setMargin(int spacingType, float margin)
		{
			SetMargin((SpacingType)spacingType, margin);
		}

		public float GetMargin(SpacingType spacingType)
		{
			return style.margin.GetRaw(spacingType);
		}

		public float GetEffectiveMargin(SpacingType spacingType)
		{
			return style.margin.Get(spacingType);
		}

		public void SetPadding(SpacingType spacingType, float padding)
		{
			if (style.padding.set((int)spacingType, padding))
				dirty();
		}

		internal void setPadding(int spacingType, float padding)
		{
			SetPadding((SpacingType) spacingType, padding);
		}

		public float GetPadding(SpacingType spacingType)
		{
			return style.padding.GetRaw(spacingType);
		}

		public float GetEffectivePadding(SpacingType spacingType)
		{
			return style.padding.Get(spacingType);
		}

		public void SetBorder(SpacingType spacingType, float border)
		{
			if (style.border.set((int)spacingType, border))
				dirty();
		}

		internal void setBorder(int spacingType, float border)
		{
			SetBorder((SpacingType) spacingType, border);
		}

		public float GetBorder(SpacingType spacingType)
		{
			return style.border.GetRaw(spacingType);
		}

		public float GetEffectiveBorder(SpacingType spacingType)
		{
			return style.border.Get(spacingType);
		}

		public float PositionTop
		{
			get { return style.positionTop; }
			set { updateFloatValue(ref style.positionTop, value); }
		}

		public float PositionBottom
		{
			get { return style.positionBottom; }
			set { updateFloatValue(ref style.positionBottom, value); }
		}

		public float PositionLeft
		{
			get { return style.positionLeft; }
			set { updateFloatValue(ref style.positionLeft, value); }
		}

		public float PositionRight
		{
			get { return style.positionRight; }
			set { updateFloatValue(ref style.positionRight, value); }
		}

		public float StyleWidth
		{
			get { return Width; }
			set { Width = value; }
		}

		public float Width
		{
			get { return style.width; }
			set { updateFloatValue(ref style.width, value); }
		}

		public float StyleHeight
		{
			get { return Height; }
			set { Height = value; }
		}

		public float Height
		{
			get { return style.height; }
			set { updateFloatValue(ref style.height, value); }
		}

		public float MinWidth
		{
			get { return style.minWidth; }
			set { updateFloatValue(ref style.minWidth, value); }
		}

		public float MinHeight
		{
			get { return style.minHeight; }
			set { updateFloatValue(ref style.minHeight, value); }
		}

		public float MaxWidth
		{
			get { return style.maxWidth; }
			set { updateFloatValue(ref style.maxWidth, value); }
		}

		public float MaxHeight
		{
			get { return style.maxHeight; }
			set { updateFloatValue(ref style.maxHeight, value); }
		}

		void updateDiscreteValue<ValueT>(ref ValueT valueRef, ValueT newValue)
		{
			if (valuesEqual(valueRef, newValue))
				return;

			valueRef = newValue;
			dirty();
		}

		void updateFloatValue(ref float valueRef, float newValue)
		{
			if (valuesEqual(valueRef, newValue))
				return;
			valueRef = newValue;
			dirty();
		}

		public float LayoutX { get { return layout.Left; } }
		public float LayoutY { get { return layout.Top; } }
		public float LayoutWidth { get { return layout.Width; } }
		public float LayoutHeight { get { return layout.Height; } }


		/**
		 * Get this node's padding, as defined by style + default padding.
		 */

		public Spacing Padding { get { return style.padding; } }

		/**
		 * Set a default padding (left/top/right/bottom) for this node.
		 */
		public void SetDefaultPadding(SpacingType spacingType, float padding)
		{
			if (style.padding.setDefault((int) spacingType, padding))
				dirty();
		}
	}

	internal static class CSSNodeExtensions
	{
		public static CSSNode getParent(this CSSNode node)
		{
			return node.Parent;
		}

		public static int getChildCount(this CSSNode node)
		{
			return node.ChildCount;
		}

		public static CSSNode getChildAt(this CSSNode node, int i)
		{
			return node[i];
		}

		public static void addChildAt(this CSSNode node, CSSNode child, int i)
		{
			node.InsertChild(i, child);
		}

		public static void removeChildAt(this CSSNode node, int i)
		{
			node.RemoveChildAt(i);
		}

		public static void setMeasureFunction(this CSSNode node, MeasureFunction measureFunction)
		{
			node.MeasureFunction = measureFunction;
		}

		public static void calculateLayout(this CSSNode node)
		{
			node.CalculateLayout();
		}

		public static bool isDirty(this CSSNode node)
		{
			return node.IsDirty;
		}
	}
}
