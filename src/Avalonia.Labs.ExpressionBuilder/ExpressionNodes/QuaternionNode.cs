
using System.Numerics;

namespace Avalonia.Labs.ExpressionBuilder
{
    // Ignore warning: 'QuaternionNode' defines operator == or operator != but does not override Object.Equals(object o) && Object.GetHashCode()
#pragma warning disable CS0660, CS0661
    /// <summary>
    /// Class QuaternionNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Avalonia.Labs.ExpressionBuilder.ExpressionNode" />
    public sealed class QuaternionNode : ExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionNode"/> class.
        /// </summary>
        internal QuaternionNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionNode"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal QuaternionNode(Quaternion value)
        {
            _value = value;
            NodeType = ExpressionNodeType.ConstantValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        internal QuaternionNode(string paramName)
        {
            ParamName = paramName;
            NodeType = ExpressionNodeType.ConstantParameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuaternionNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        internal QuaternionNode(string paramName, Quaternion value)
        {
            ParamName = paramName;
            _value = value;
            NodeType = ExpressionNodeType.ConstantParameter;

            SetQuaternionParameter(paramName, value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Quaternion"/> to <see cref="QuaternionNode"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator QuaternionNode(Quaternion value)
        {
            return new QuaternionNode(value);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static QuaternionNode operator *(QuaternionNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<QuaternionNode>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static QuaternionNode operator *(QuaternionNode left, QuaternionNode right)
        {
            return ExpressionFunctions.Function<QuaternionNode>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the / operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static QuaternionNode operator /(QuaternionNode left, QuaternionNode right)
        {
            return ExpressionFunctions.Function<QuaternionNode>(ExpressionNodeType.Divide, left, right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator ==(QuaternionNode left, QuaternionNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.Equals, left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator !=(QuaternionNode left, QuaternionNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.NotEquals, left, right);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.String.</returns>
        protected internal override string GetValue()
        {
            return $"Quaternion({_value.X.ToCompositionString()},{_value.Y.ToCompositionString()},{_value.Z.ToCompositionString()},{_value.W.ToCompositionString()})";
        }

        private Quaternion _value;
    }
#pragma warning restore CS0660, CS0661
}
