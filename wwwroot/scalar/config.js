// wwwroot/scalar/config.js
export default {
	// Custom slug generation for operations
	generateOperationSlug: (operation) => `custom-${operation.method.toLowerCase()}${operation.path}`,
	// Hook into document selection events
	onDocumentSelect: () => console.log('Document changed'),
	// Add any other custom configuration options supported by Scalar
	// Checkout https://guides.scalar.com/scalar/scalar-api-references/configuration
}