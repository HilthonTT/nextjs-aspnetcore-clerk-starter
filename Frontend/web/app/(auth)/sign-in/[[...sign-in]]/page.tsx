import type { Metadata } from "next";
import { SignIn } from "@clerk/nextjs";

export const metadata: Metadata = { title: "Sign in" };

const SignInPage = () => <SignIn />;

export default SignInPage;
