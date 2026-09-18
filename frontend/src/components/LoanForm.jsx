import { useState } from "react";

function LoanForm({ onSubmit, loading }) {
    const [formData, setFormData] = useState({
        loanAmount: "",
        assetValue: "",
        creditScore: "",
    });

    const [errors, setErrors] = useState({});

    function handleChange(event) {
        const { name, value } = event.target;

        setFormData((current) => ({
            ...current,
            [name]: value,
        }));

        setErrors((current) => ({
            ...current,
            [name]: "",
        }));
    }

    function validate() {
        const newErrors = {};

        const loanAmount = Number(formData.loanAmount);
        const assetValue = Number(formData.assetValue);
        const creditScore = Number(formData.creditScore);

        if (!formData.loanAmount || loanAmount <= 0) {
            newErrors.loanAmount = "Loan amount must be greater than zero.";
        }

        if (!formData.assetValue || assetValue <= 0) {
            newErrors.assetValue = "Asset value must be greater than zero.";
        }

        if (
            !formData.creditScore ||
            creditScore < 1 ||
            creditScore > 999
        ) {
            newErrors.creditScore = "Credit score must be between 1 and 999.";
        }

        setErrors(newErrors);

        return Object.keys(newErrors).length === 0;
    }

    function handleSubmit(event) {
        event.preventDefault();

        if (!validate()) {
            return;
        }

        onSubmit({
            loanAmount: Number(formData.loanAmount),
            assetValue: Number(formData.assetValue),
            creditScore: Number(formData.creditScore),
        });
    }

    return (
        <form
            onSubmit={handleSubmit}
            noValidate
            className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm"
        >
            <div className="space-y-5">
                <div>
                    <label
                        htmlFor="loanAmount"
                        className="mb-2 block text-sm font-semibold text-slate-700"
                    >
                        Loan Amount
                    </label>

                    <div className="relative">
                        <span
                            aria-hidden="true"
                            className="absolute left-4 top-1/2 -translate-y-1/2 text-sm font-medium text-slate-500"
                        >
                            £
                        </span>

                        <input
                            id="loanAmount"
                            name="loanAmount"
                            type="number"
                            required
                            min="0"
                            step="0.01"
                            inputMode="decimal"
                            value={formData.loanAmount}
                            onChange={handleChange}
                            placeholder="500000"
                            aria-invalid={Boolean(errors.loanAmount)}
                            aria-describedby={
                                errors.loanAmount
                                    ? "loanAmount-error"
                                    : undefined
                            }
                            className={`w-full rounded-xl border py-3 pl-9 pr-4 text-slate-900 placeholder:text-slate-400 outline-none transition focus-visible:ring-2 focus-visible:ring-offset-1 ${
                                errors.loanAmount
                                    ? "border-red-400 focus-visible:border-red-500 focus-visible:ring-red-500/20"
                                    : "border-slate-300 focus-visible:border-slate-900 focus-visible:ring-slate-900/15"
                            }`}
                        />
                    </div>

                    {errors.loanAmount && (
                        <p
                            id="loanAmount-error"
                            role="alert"
                            className="mt-2 text-sm text-red-600"
                        >
                            {errors.loanAmount}
                        </p>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="assetValue"
                        className="mb-2 block text-sm font-semibold text-slate-700"
                    >
                        Asset Value
                    </label>

                    <div className="relative">
                        <span
                            aria-hidden="true"
                            className="absolute left-4 top-1/2 -translate-y-1/2 text-sm font-medium text-slate-500"
                        >
                            £
                        </span>

                        <input
                            id="assetValue"
                            name="assetValue"
                            type="number"
                            required
                            min="0"
                            step="0.01"
                            inputMode="decimal"
                            value={formData.assetValue}
                            onChange={handleChange}
                            placeholder="1000000"
                            aria-invalid={Boolean(errors.assetValue)}
                            aria-describedby={
                                errors.assetValue
                                    ? "assetValue-error"
                                    : undefined
                            }
                            className={`w-full rounded-xl border py-3 pl-9 pr-4 text-slate-900 placeholder:text-slate-400 outline-none transition focus-visible:ring-2 focus-visible:ring-offset-1 ${
                                errors.assetValue
                                    ? "border-red-400 focus-visible:border-red-500 focus-visible:ring-red-500/20"
                                    : "border-slate-300 focus-visible:border-slate-900 focus-visible:ring-slate-900/15"
                            }`}
                        />
                    </div>

                    {errors.assetValue && (
                        <p
                            id="assetValue-error"
                            role="alert"
                            className="mt-2 text-sm text-red-600"
                        >
                            {errors.assetValue}
                        </p>
                    )}
                </div>

                <div>
                    <label
                        htmlFor="creditScore"
                        className="mb-2 block text-sm font-semibold text-slate-700"
                    >
                        Credit Score
                    </label>

                    <input
                        id="creditScore"
                        name="creditScore"
                        type="number"
                        required
                        min="1"
                        max="999"
                        step="1"
                        inputMode="numeric"
                        value={formData.creditScore}
                        onChange={handleChange}
                        placeholder="800"
                        aria-invalid={Boolean(errors.creditScore)}
                        aria-describedby={
                            errors.creditScore
                                ? "creditScore-error"
                                : undefined
                        }
                        className={`w-full rounded-xl border px-4 py-3 text-slate-900 placeholder:text-slate-400 outline-none transition focus-visible:ring-2 focus-visible:ring-offset-1 ${
                            errors.creditScore
                                ? "border-red-400 focus-visible:border-red-500 focus-visible:ring-red-500/20"
                                : "border-slate-300 focus-visible:border-slate-900 focus-visible:ring-slate-900/15"
                        }`}
                    />

                    {errors.creditScore && (
                        <p
                            id="creditScore-error"
                            role="alert"
                            className="mt-2 text-sm text-red-600"
                        >
                            {errors.creditScore}
                        </p>
                    )}
                </div>

                <button
                    type="submit"
                    disabled={loading}
                    aria-busy={loading}
                    className="inline-flex w-full items-center justify-center rounded-xl bg-slate-900 px-5 py-3.5 text-sm font-semibold text-white shadow-sm transition hover:bg-slate-800 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-slate-900 focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-60"
                >
                    {loading ? (
                        <>
                            <svg
                                className="mr-2.5 h-4 w-4 animate-spin text-white"
                                xmlns="http://www.w3.org/2000/svg"
                                fill="none"
                                viewBox="0 0 24 24"
                                aria-hidden="true"
                            >
                                <circle
                                    className="opacity-25"
                                    cx="12"
                                    cy="12"
                                    r="10"
                                    stroke="currentColor"
                                    strokeWidth="4"
                                />
                                <path
                                    className="opacity-75"
                                    fill="currentColor"
                                    d="M4 12a8 8 0 018-8v4a4 4 0 00-4 4H4z"
                                />
                            </svg>
                            <span>Evaluating Application...</span>
                        </>
                    ) : (
                        "Evaluate Application"
                    )}
                </button>
            </div>
        </form>
    );
}

export default LoanForm;