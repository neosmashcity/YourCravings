window.emailJsHelper = {
    sendEmail: async function (
        serviceId,
        templateId,
        publicKey,
        templateParams
    ) {
        const response = await emailjs.send(
            serviceId,
            templateId,
            templateParams,
            {
                publicKey: publicKey
            }
        );

        return response.status === 200;
    }
};
