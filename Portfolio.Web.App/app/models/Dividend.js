/////////////////////////////////////////////////////////////
// Dividend Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.Dividend = function (data, investment) {
	var self = this;

	if (data) {
		_.extend(this, data);

		// set pointer to parent investment
		this.investment = investment;
	};
};


// Dividend class methods
c3o.Core.Data.Dividend.prototype = {

	Total: function () {
		return this.dividend * this.investment.quantity;
	}

	//// Merge Additional product data into existing object
	//Merge: function (Dividend) {
	//	_.extend(this, investment);
	//	//this.summary = product.summary;
	//	//this.description = product.description;
	//	//this.content = product.content;
	//	//this.mediaObjects = product.mediaObjects;
	//}

	////get Ordered() { return this.orders && this.orders.length > 0; },
	////get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};