window.historyApiHelper = {
    async post(apiUrl, request) {
        try {
            const response = await fetch(apiUrl, {
                method: "POST",
                headers: {
                    "Content-Type": "text/plain;charset=UTF-8"
                },
                body: JSON.stringify(request)
            });

            if (!response.ok) {
                throw new Error(`HTTP error: ${response.status}`);
            }

            return await response.text();
        }
        catch (error) {
            console.error("History API Error:", error);
            throw error;
        }
    }
};
