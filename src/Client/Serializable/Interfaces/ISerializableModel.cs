// <copyright file="ISerializableModel.cs" company="WillowTree, LLC">
// Copyright (c) WillowTree, LLC. All rights reserved.
// </copyright>

namespace WillowTree.Sweetgum.Client.Serializable.Interfaces
{
    /// <summary>
    /// A serializable model that can be converted to a model.
    /// </summary>
    /// <typeparam name="TSerializableModel">The serializable model type.</typeparam>
    /// <typeparam name="TModel">The model type.</typeparam>
    public interface ISerializableModel<TSerializableModel, TModel>
        where TSerializableModel : ISerializableModel<TSerializableModel, TModel>
        where TModel : IModel<TModel, TSerializableModel>
    {
        /// <summary>
        /// Converts an instance of <typeparamref name="TSerializableModel"/> to an instance of <typeparamref name="TModel"/>.
        /// </summary>
        /// <returns>An instance of <typeparamref name="TModel"/>.</returns>
        TModel ToModel();
    }
}
