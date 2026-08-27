import type { Metadata } from "next";
import { SignUp } from "@clerk/nextjs";

export const metadata: Metadata = { title: "Sign up" };

const SignUpPage = () => <SignUp />;

export default SignUpPage;
