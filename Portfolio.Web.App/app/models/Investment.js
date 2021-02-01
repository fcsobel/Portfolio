
c3o.Core.Data.InvestmentResponse = function (data) {
	var self = this;
	if (data) {
		this.accountId = data.accountId;
		this.investmentId = data.investmentId;
		this.name = data.name;
		this.quantity = data.quantity;
		//this.cost = data.cost;
		this.stockId = data.stockId;
		this.ticker = data.ticker;
		//this.price = data.price;
		//this.dividend = data.dividend;
		this.tags = data.tags;
	};
};


/////////////////////////////////////////////////////////////
// Investment Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.Investment = function (data) {
	var self = this;

	if (data) {
		_.extend(this, data);

		// Transform Account Investments
		if (this.dividends != null && this.dividends != undefined) {
			this.dividends = $.map(this.dividends, function (item, i) {
				return new c3o.Core.Data.Dividend(item, self);
			});
		}
	};
};


// Investment class methods
c3o.Core.Data.Investment.prototype = {

	Price: function () {
			return this.price * this.quantity;
	},
	Income: function () {
		return (this.price * this.quantity * (this.dividend / 100));
	},


	//// Merge Additional product data into existing object
	//Merge: function (investment) {
	//	_.extend(this, investment);
	//	//this.summary = product.summary;
	//	//this.description = product.description;
	//	//this.content = product.content;
	//	//this.mediaObjects = product.mediaObjects;
	//}

	////get Ordered() { return this.orders && this.orders.length > 0; },
	////get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};